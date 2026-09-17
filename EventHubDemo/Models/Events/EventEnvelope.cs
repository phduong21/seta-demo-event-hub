namespace EventHubDemo.Models.Events;

public class EventEnvelope<T>
{
    public Guid EventId { get; set; }
    public string EventType { get; set; } = string.Empty;
    public DateTimeOffset OccurredAt { get; set; }
    public T Payload { get; set; } = default!;

    public static EventEnvelope<T> Create(string eventType, T payload)
    {
        return new EventEnvelope<T>
        {
            EventId = Guid.NewGuid(),
            EventType = eventType,
            OccurredAt = DateTimeOffset.UtcNow,
            Payload = payload,
        };
    }
}
