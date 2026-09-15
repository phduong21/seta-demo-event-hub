using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using EventHubDemo.Configuration;
using EventHubDemo.Interfaces;
using EventHubDemo.Models;
using Microsoft.Extensions.Options;

namespace EventHubDemo.Services;

public class EventHubDiagnosticsService : IEventHubDiagnosticsService
{
    private readonly EventHubProducerClient _producer;
    private readonly EventHubOptions _options;
    private readonly ILogger<EventHubDiagnosticsService> _logger;

    public EventHubDiagnosticsService(
        EventHubProducerClient producer,
        IOptions<EventHubOptions> options,
        ILogger<EventHubDiagnosticsService> logger)
    {
        _producer = producer;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SendResult> TestSendAsync(CancellationToken cancellationToken)
    {
        var pingId = Guid.NewGuid().ToString();

        try
        {
            await _producer.SendAsync([new EventData(pingId)], cancellationToken);
            return new SendResult(true, pingId, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Diagnostic send test failed.");
            return new SendResult(false, pingId, ex.Message);
        }
    }

    public async Task<ReceiveResult> TestReceiveAsync(CancellationToken cancellationToken)
    {
        await using var consumer = new EventHubConsumerClient(_options.ConsumerGroup, _options.ConnectionString);

        try
        {
            var partitionIds = await consumer.GetPartitionIdsAsync(cancellationToken);
            return new ReceiveResult(true, partitionIds.Length, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Diagnostic receive test failed.");
            return new ReceiveResult(false, 0, ex.Message);
        }
    }
}
