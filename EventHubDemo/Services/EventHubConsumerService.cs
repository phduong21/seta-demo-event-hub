using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;

namespace EventHubDemo.Services;

public class EventHubConsumerService : BackgroundService
{
    private readonly EventProcessorClient _processor;
    private readonly EventDispatcher _dispatcher;
    private readonly ILogger<EventHubConsumerService> _logger;

    public EventHubConsumerService(
        EventProcessorClient processor,
        EventDispatcher dispatcher,
        ILogger<EventHubConsumerService> logger)
    {
        _processor = processor;
        _dispatcher = dispatcher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stopToken)
    {
        _processor.PartitionInitializingAsync += OnPartitionInitializing;
        _processor.ProcessEventAsync += OnProcessEvent;
        _processor.ProcessErrorAsync += OnProcessError;

        await _processor.StartProcessingAsync(stopToken);
        _logger.LogInformation("Consumer started");

        try
        {
            await Task.Delay(Timeout.Infinite, stopToken);
        }
        catch (OperationCanceledException)
        {
        }

        await _processor.StopProcessingAsync();
        _logger.LogInformation("Consumer stopped");
    }

    private Task OnPartitionInitializing(PartitionInitializingEventArgs args)
    {
        args.DefaultStartingPosition = EventPosition.Latest;
        return Task.CompletedTask;
    }

    private async Task OnProcessEvent(ProcessEventArgs args)
    {
        await _dispatcher.DispatchAsync(args.Data, args.Partition.PartitionId, args.CancellationToken);
        await args.UpdateCheckpointAsync(args.CancellationToken);
    }

    private Task OnProcessError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Processor error on partition {PartitionId}", args.PartitionId);
        return Task.CompletedTask;
    }
}
