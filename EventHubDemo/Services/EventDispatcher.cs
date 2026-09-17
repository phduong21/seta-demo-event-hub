using Azure.Messaging.EventHubs;
using EventHubDemo.Models.Events;

namespace EventHubDemo.Services;

public class EventDispatcher
{
    private readonly ILogger<EventDispatcher> _logger;

    public EventDispatcher(ILogger<EventDispatcher> logger)
    {
        _logger = logger;
    }

    public Task DispatchAsync(EventData eventData, string partitionId, CancellationToken cancellationToken)
    {
        eventData.Properties.TryGetValue("eventType", out var eventType);
        eventData.Properties.TryGetValue("eventId", out var eventId);
        var body = eventData.EventBody.ToString();

        switch (eventType as string)
        {
            case EventTypes.OrderCreated:
            case EventTypes.OrderProcessing:
            case EventTypes.OrderCompleted:
            case EventTypes.OrderCancelled:
                _logger.LogInformation("Handled {EventType} EventId={EventId} Partition={PartitionId} Sequence={SequenceNumber} Body={Body}",
                    eventType, eventId, partitionId, eventData.SequenceNumber, body);
                break;
            default:
                _logger.LogWarning("Unknown event type {EventType} on partition {PartitionId}", eventType, partitionId);
                break;
        }

        return Task.CompletedTask;
    }
}
