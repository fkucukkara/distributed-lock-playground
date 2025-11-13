# DistributedLock101 - Copilot Instructions

## Project Overview

This is a .NET Aspire playground demonstrating three distributed locking strategies:
1. **PostgreSQL Advisory Locks** (`/distributed-lock-pg`) - Using native `pg_try_advisory_lock`
2. **Redis Raw Implementation** (`/distributed-lock-redis`) - Manual lock with Lua script for safe release
3. **Redis with Medallion.Threading** (`/distributed-lock-redis-third-party`) - Using established library patterns

The project showcases trade-offs between hand-rolled vs. library-based approaches in a microservices context.

## Architecture

**Aspire Orchestration Model:**
- `DistributedLock101.AppHost` is the orchestrator - defines infrastructure dependencies (PostgreSQL, Redis) and service topology
- `DistributedLock101.ApiService` is the API service - references infrastructure via Aspire's service discovery (`builder.AddNpgsqlDataSource("distributedlock")`, `builder.AddRedisClient("cache")`)
- `DistributedLock101.ServiceDefaults` provides shared cross-cutting concerns (OpenTelemetry, health checks, resilience handlers)

**To run the application:** Always start via AppHost (`dotnet run --project DistributedLock101.AppHost`), not individual services. AppHost spins up Postgres/Redis containers and wires up service references.

## Key Patterns & Conventions

### Distributed Lock Implementations

**PostgreSQL Advisory Locks Pattern:**
```csharp
var key = HashKey("my-lock-key"); // Hash string to int64
var acquired = await conn.ExecuteScalarAsync<bool>("SELECT pg_try_advisory_lock(@key);", new { key });
// ... critical section ...
await conn.ExecuteAsync("SELECT pg_advisory_unlock(@key);", new { key });
```
- Keys are SHA256-hashed to int64 for PostgreSQL compatibility
- Locks are connection-scoped - must keep connection open during lock lifetime
- No automatic timeout - implement application-level timeout logic

**Redis Raw Lock Pattern:**
```csharp
var lockValue = Guid.NewGuid().ToString(); // Unique token to prevent releasing others' locks
var acquired = await db.StringSetAsync(lockKey, lockValue, expiry, When.NotExists);
// ... critical section ...
// Use Lua script for atomic check-and-delete:
const string releaseLockScript = @"
    if redis.call('get', KEYS[1]) == ARGV[1] then
        return redis.call('del', KEYS[1])
    else
        return 0
    end";
```
- Always use unique lock values (GUID) to safely release only your own lock
- Lua scripts ensure atomicity - critical for preventing race conditions on release
- Set expiry to prevent deadlocks if process crashes before unlock

**Medallion.Threading Pattern:**
```csharp
var lockHandle = await distributedLockProvider.TryAcquireLockAsync("report-lock");
if (lockHandle is null) return Results.Conflict("Lock unavailable");
using (lockHandle) { /* critical section */ }
```
- Library handles complexities (timeouts, retries, safe release)
- Prefer this for production - less error-prone than raw implementations

### Aspire Service Integration

**Adding Infrastructure:**
```csharp
// In AppHost.cs
var db = builder.AddPostgres("postgres").AddDatabase("distributedlock");
var cache = builder.AddRedis("cache");
```

**Consuming in Services:**
```csharp
// In Program.cs
builder.AddNpgsqlDataSource("distributedlock"); // Name matches AppHost database name
builder.AddRedisClient("cache"); // Name matches AppHost cache name
```

Names must match exactly between AppHost and consuming services. Aspire injects connection strings automatically.

### Service Defaults Pattern

All services must call `builder.AddServiceDefaults()` first - this configures:
- OpenTelemetry tracing/metrics (excludes `/health` and `/alive` endpoints)
- Service discovery with HTTP resilience handlers
- Health checks endpoint mapping via `app.MapDefaultEndpoints()`

Health endpoints (`/health`, `/alive`) only exposed in Development environment for security.

## Development Workflow

**Build & Run:**
```powershell
dotnet restore
dotnet run --project DistributedLock101.AppHost
```

**Testing Endpoints:**
Use `DistributedLock101.ApiService.http` for HTTP client testing in VS Code. Endpoints simulate 15-second critical sections to demonstrate lock behavior under concurrent requests.

**Adding New Lock Strategies:**
1. Register provider in `Program.cs` (singleton for stateful providers like Medallion)
2. Create endpoint following existing patterns (`/distributed-lock-{strategy}`)
3. Add test case to `.http` file
4. Document trade-offs in README

## Target Framework & Dependencies

- **SDK:** .NET 10.0 (global.json specifies `10.0.100` with preview flag)
- **Aspire:** Version 13.0.0 for hosting/integration packages
- **Critical Libraries:**
  - `Medallion.Threading` 2.7.0 - Production-grade distributed locks
  - `Dapper` 2.1.66 - Lightweight ORM for PostgreSQL advisory lock queries
  - `Aspire.Npgsql` / `Aspire.StackExchange.Redis` - Aspire integrations

## Common Gotchas

1. **Don't call services directly** - Always run via AppHost or dependencies won't resolve
2. **Lock key collisions** - Use meaningful, unique prefixes (`lock:`, resource identifiers) to avoid unintended blocking
3. **PostgreSQL lock lifetime** - Advisory locks released on connection close, not transaction commit
4. **Redis lock expiry** - Set expiry > expected critical section duration to prevent premature unlock
5. **Lua script necessity** - Direct Redis DEL without value check can release another process's lock
