using Dapper;
using Medallion.Threading;
using Medallion.Threading.Redis;
using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.AddNpgsqlDataSource("distributedlock");
builder.AddRedisClient("cache");

builder.Services.AddSingleton<IDistributedLockProvider>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisDistributedSynchronizationProvider(redis.GetDatabase());
});

builder.Services.AddProblemDetails();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/distributed-lock-pg", async (NpgsqlDataSource dataSource) =>
{
    await using var conn = await dataSource.OpenConnectionAsync();

    var key = HashKey("my-distributed-lock-key");
    var acquired = await conn.ExecuteScalarAsync<bool>("SELECT pg_try_advisory_lock(@key);", new { key });

    if (acquired is false)
        return Results.Conflict("Could not acquire the distributed lock.");

    try
    {
        await DoWork();
    }
    finally
    {
        await conn.ExecuteAsync("SELECT pg_advisory_unlock(@key);", new { key });
    }

    static long HashKey(string key) =>
    BitConverter.ToInt64(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(key)), 0);

    static Task DoWork() => Task.Delay(15000);

    return Results.Ok();
});

app.MapGet("/distributed-lock-redis", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var lockKey = "lock:my-distributed-lock-key";
    var lockValue = Guid.NewGuid().ToString();
    var lockExpiry = TimeSpan.FromSeconds(30);

    var acquired = await db.StringSetAsync(
      lockKey,
      lockValue,
      lockExpiry,
      When.NotExists
  );

    if (acquired is false)
        return Results.Conflict("Could not acquire the distributed lock.");

    try
    {
        await DoWork();
    }
    finally
    {
        await ReleaseLockSafely(db, lockKey, lockValue);
    }

    static Task DoWork() => Task.Delay(15000);

    static async Task ReleaseLockSafely(IDatabase db, string lockKey, string lockValue)
    {
        // Lua script ensures atomic check-and-delete to prevent releasing someone else's lock
        const string releaseLockScript = @"
        if redis.call('get', KEYS[1]) == ARGV[1] then
            return redis.call('del', KEYS[1])
        else
            return 0
        end";

        await db.ScriptEvaluateAsync(
            releaseLockScript,
            new RedisKey[] { lockKey },
            new RedisValue[] { lockValue }
        );
    }

    return Results.Ok();
});

app.MapGet("/distributed-lock-redis-third-party", async (IDistributedLockProvider distributedLockProvider) =>
{
    var lockHandle = await distributedLockProvider.TryAcquireLockAsync("report-lock");
    if (lockHandle is null)
    {
        return Results.Conflict("Could not acquire the distributed lock.");
    }

    using (lockHandle)
    {
        await Task.Delay(15000);
    }

    return Results.Ok();
});

app.MapDefaultEndpoints();

app.Run();
