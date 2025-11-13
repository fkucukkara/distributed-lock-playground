# DistributedLock101

A playground API and sample solution demonstrating distributed locking concepts in .NET.

## Overview

DistributedLock101 is a sample project that explores distributed lock mechanisms in a microservices environment. It includes multiple services and demonstrates how to coordinate access to shared resources using distributed locks.

## What is a Distributed Lock?

A distributed lock is a mechanism that ensures only one process or service in a distributed system can access a shared resource at a time. This is crucial for maintaining data consistency and preventing race conditions in microservices, cloud-native, or clustered environments.

## Use Cases for Distributed Locks

- **Resource Coordination:** Prevent multiple services from modifying the same resource simultaneously (e.g., updating a user's balance in a banking system).
- **Leader Election:** Ensure only one instance of a service acts as the leader at any time.
- **Job Scheduling:** Avoid duplicate job execution in distributed background workers.
- **Rate Limiting:** Enforce limits on how often a resource can be accessed across distributed nodes.
- **Transactional Operations:** Coordinate multi-step operations that must not overlap.

## Project Structure

- `DistributedLock101.ApiService/` — ASP.NET Core Web API exposing endpoints to demonstrate distributed lock usage.
- `DistributedLock101.AppHost/` — Host application for running background jobs or services.
- `DistributedLock101.ServiceDefaults/` — Shared service configuration and extensions.
- `DistributedLock101.Web/` — (Optional) Web frontend for interacting with the API.

## Getting Started

1. **Clone the repository:**

   ```sh
   git clone https://github.com/fkucukkara/distributed-lock-playground.git
   cd DistributedLock101
   ```

2. **Restore dependencies:**

   ```sh
   dotnet restore
   ```

3. **Run the API service:**

   ```sh
   dotnet run --project DistributedLock101.ApiService
   ```

4. **Explore the API:**
   Use tools like Postman or Swagger UI to interact with the endpoints.

## Technologies Used

- .NET 10
- ASP.NET Core
- (Add your distributed lock provider, e.g., Redis, SQL, etc.)

## Example Distributed Lock Providers

- Redis (using RedLock algorithm)
- SQL Server (using sp_getapplock)
- Azure Blob Lease
- Consul

## License

[![MIT License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

This project is licensed under the MIT License. See the [`LICENSE`](LICENSE) file for details.
