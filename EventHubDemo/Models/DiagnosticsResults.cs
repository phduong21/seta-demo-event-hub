namespace EventHubDemo.Models;

public sealed record SendResult(bool Ok, string? PingId, string? Error);

public sealed record ReceiveResult(bool Ok, int PartitionCount, string? Error);
