using System.Text.Json;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using EventHubDemo.Interfaces;
using EventHubDemo.Models.Events;
using EventHubDemo.Models.Responses;

namespace EventHubDemo.Services;

public class EventHubPublisherService : IEventPublisherService
{
    private readonly EventHubProducerClient _producer;
    private readonly ILogger<EventHubPublisherService> _logger;

    public EventHubPublisherService(EventHubProducerClient producer, ILogger<EventHubPublisherService> logger)
    {
        _producer = producer;
        _logger = logger;
    }

    public async Task<ApiResponse<PublishResponse>> PublishAsync<T>(EventEnvelope<T> envelope, string partitionKey, CancellationToken cancellationToken)
    {
        try
        {
            var eventData = new EventData(JsonSerializer.SerializeToUtf8Bytes(envelope))
            {
                ContentType = "application/json",
            };

            eventData.Properties["eventType"] = envelope.EventType;
            eventData.Properties["eventId"] = envelope.EventId.ToString();
            eventData.Properties["correlationId"] = envelope.CorrelationId;

            await _producer.SendAsync(new[] { eventData }, new SendEventOptions { PartitionKey = partitionKey }, cancellationToken);

            _logger.LogInformation("Published {EventType} EventId = {EventId} PartitionKey = {PartitionKey}", envelope.EventType, envelope.EventId, partitionKey);

            return ApiResponse<PublishResponse>.Ok(new PublishResponse { EventId = envelope.EventId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish {EventType} EventId = {EventId}", envelope.EventType, envelope.EventId);
            return ApiResponse<PublishResponse>.Fail(ex.Message);
        }
    }
}
