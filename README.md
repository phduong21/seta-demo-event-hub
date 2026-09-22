# EventHubDemo

A small Pub/Sub demo on Azure Event Hubs. An API publishes order events, a background
consumer reads them with durable checkpointing and idempotent processing.

# Stack
- .NET 10, ASP.NET Core (Controllers)
- Azure Event Hubs (Azure.Messaging.EventHubs)
- PostgreSQL + EF Core — idempotency store
- Azurite — local Blob for checkpoints


# Prerequisite
- .NET 10 SDK
- PostgreSQL running, with a database named `eventhubdemo`
- Azurite (npm install -g azurite)
- An Azure Event Hub + its connection string

# Setting
Create `EventHubDemo/appsettings.Development.json` (gitignored — holds secrets):
{
  "EventHub": { "ConnectionString": "<your Event Hub connection string>" },
  "Storage": { "ConnectionString": "UseDevelopmentStorage=true" },
  "ConnectionStrings": { "Postgres": "Host=localhost;Port=5432;Database=eventhubdemo;Username=<your credential>" }
}


# Run
azurite --silent --location ~/azurite-data

cd EventHubDemo && ASPNETCORE_ENVIRONMENT=Development dotnet run

Swagger: http://localhost:5293/swagger. 

# Test
cd EventHubDemo.Tests && dotnet test

