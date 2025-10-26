var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres")
    .AddDatabase("distributedlock");

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.DistributedLock101_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(db)
    .WaitFor(db)
    .WithReference(cache)
    .WaitFor(cache);

builder.Build().Run();
