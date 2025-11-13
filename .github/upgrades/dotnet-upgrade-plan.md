# .NET 10 Upgrade Plan

## Execution Steps

Execute steps below sequentially one by one in the order they are listed.

1. Validate that a .NET 10 SDK required for this upgrade is installed on the machine and if not, help to get it installed.
2. Ensure that the SDK version specified in global.json files is compatible with the .NET 10 upgrade.
3. Upgrade DistributedLock101.ServiceDefaults\DistributedLock101.ServiceDefaults.csproj
4. Upgrade DistributedLock101.ApiService\DistributedLock101.ApiService.csproj
5. Upgrade DistributedLock101.AppHost\DistributedLock101.AppHost.csproj

## Settings

This section contains settings and data used by execution steps.

### Aggregate NuGet packages modifications across all projects

NuGet packages used across all selected projects or their dependencies that need version update in projects that reference them.

| Package Name                                  | Current Version | New Version | Description                                   |
|:----------------------------------------------|:---------------:|:-----------:|:----------------------------------------------|
| Aspire.Hosting.AppHost                        | 9.5.0           | 13.0.0      | Recommended for .NET 10                       |
| Aspire.Hosting.PostgreSQL                     | 9.5.2           | 13.0.0      | Recommended for .NET 10                       |
| Aspire.Hosting.Redis                          | 9.5.0           | 13.0.0      | Recommended for .NET 10                       |
| Aspire.Npgsql                                 | 9.5.2           | 13.0.0      | Recommended for .NET 10                       |
| Aspire.StackExchange.Redis                    | 9.5.2           | 13.0.0      | Recommended for .NET 10                       |
| Microsoft.AspNetCore.OpenApi                  | 9.0.9           | 10.0.0      | Recommended for .NET 10                       |
| Microsoft.Extensions.Http.Resilience          | 9.9.0           | 10.0.0      | Recommended for .NET 10                       |
| Microsoft.Extensions.ServiceDiscovery         | 9.5.0           | 10.0.0      | Recommended for .NET 10                       |
| OpenTelemetry.Instrumentation.AspNetCore      | 1.12.0          | 1.14.0      | Recommended for .NET 10                       |
| OpenTelemetry.Instrumentation.Http            | 1.12.0          | 1.14.0      | Recommended for .NET 10                       |

### Project upgrade details

This section contains details about each project upgrade and modifications that need to be done in the project.

#### DistributedLock101.ServiceDefaults\DistributedLock101.ServiceDefaults.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Microsoft.Extensions.Http.Resilience should be updated from `9.9.0` to `10.0.0` (*recommended for .NET 10*)
  - Microsoft.Extensions.ServiceDiscovery should be updated from `9.5.0` to `10.0.0` (*recommended for .NET 10*)
  - OpenTelemetry.Instrumentation.AspNetCore should be updated from `1.12.0` to `1.14.0` (*recommended for .NET 10*)
  - OpenTelemetry.Instrumentation.Http should be updated from `1.12.0` to `1.14.0` (*recommended for .NET 10*)

#### DistributedLock101.ApiService\DistributedLock101.ApiService.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Aspire.Npgsql should be updated from `9.5.2` to `13.0.0` (*recommended for .NET 10*)
  - Aspire.StackExchange.Redis should be updated from `9.5.2` to `13.0.0` (*recommended for .NET 10*)
  - Microsoft.AspNetCore.OpenApi should be updated from `9.0.9` to `10.0.0` (*recommended for .NET 10*)

#### DistributedLock101.AppHost\DistributedLock101.AppHost.csproj modifications

Project properties changes:
  - Target framework should be changed from `net9.0` to `net10.0`

NuGet packages changes:
  - Aspire.Hosting.AppHost should be updated from `9.5.0` to `13.0.0` (*recommended for .NET 10*)
  - Aspire.Hosting.PostgreSQL should be updated from `9.5.2` to `13.0.0` (*recommended for .NET 10*)
  - Aspire.Hosting.Redis should be updated from `9.5.0` to `13.0.0` (*recommended for .NET 10*)