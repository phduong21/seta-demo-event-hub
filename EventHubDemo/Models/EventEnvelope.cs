namespace EventHubDemo.Models;

public class EventEnvelope<T>
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public T Payload { get; set; } = default!;

    public static EventEnvelope<T> Create(string eventType, T payload, string? correlationId = null)
    {
        return new EventEnvelope<T>
        {
            EventId = Guid.NewGuid(),
            EventType = eventType,
            OccurredAt = DateTimeOffset.UtcNow,
            CorrelationId = correlationId ?? Guid.NewGuid().ToString(),
            Payload = payload,
        };
    }
}
