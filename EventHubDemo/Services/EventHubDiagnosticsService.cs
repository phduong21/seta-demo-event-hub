using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Producer;
using EventHubDemo.Configuration;
using EventHubDemo.Interfaces;
using EventHubDemo.Models.Responses;
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

    public async Task<ApiResponse<TestSendResponse>> TestSendAsync(CancellationToken cancellationToken)
    {
        var pingId = Guid.NewGuid().ToString();

        try
        {
            await _producer.SendAsync(new[] { new EventData(pingId) }, cancellationToken);
            _logger.LogInformation("Send test succeeded. PingId= {PingId}", pingId);

            return ApiResponse<TestSendResponse>.Ok(new TestSendResponse { PingId = pingId });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Send test failed.");
            return ApiResponse<TestSendResponse>.Fail(ex.Message);
        }
    }

    public async Task<ApiResponse<TestReceiveResponse>> TestReceiveAsync(CancellationToken cancellationToken)
    {
        await using var consumer = new EventHubConsumerClient(_options.ConsumerGroup, _options.ConnectionString);

        try
        {
            var partitionIds = await consumer.GetPartitionIdsAsync(cancellationToken);
            _logger.LogInformation("Receive test connected. PartitionCount = {PartitionCount} ", partitionIds.Length);

            return ApiResponse<TestReceiveResponse>.Ok(new TestReceiveResponse { PartitionCount = partitionIds.Length });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Receive test failed.");
            return ApiResponse<TestReceiveResponse>.Fail(ex.Message);
        }
    }
}
