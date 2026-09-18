using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using EventHubDemo.Interfaces;

namespace EventHubDemo.Services;

public class EventHubConsumerService : BackgroundService
{
    private readonly EventProcessorClient _processor;
    private readonly EventDispatcher _dispatcher;
    private readonly ILogger<EventHubConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;


    public EventHubConsumerService(
        EventProcessorClient processor,
        EventDispatcher dispatcher,
        ILogger<EventHubConsumerService> logger,
        IServiceScopeFactory scopeFactory
        )
    {
        _processor = processor;
        _dispatcher = dispatcher;
        _logger = logger;
        _scopeFactory = scopeFactory;

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
        await ProcessAsync(args.Data, args.Partition.PartitionId, args.CancellationToken);
        await args.UpdateCheckpointAsync(args.CancellationToken);
    }

    private async Task ProcessAsync(EventData data, string partitionId, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(GetEventProperty(data, "eventId"), out var eventId))
        {
            _logger.LogWarning("Invalid eventId on partition: {PartitionId}, sequence: {SequenceNumber}", partitionId, data.SequenceNumber);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        await HandleAsync(scope.ServiceProvider, data, partitionId, eventId, cancellationToken);
    }

    private async Task HandleAsync(IServiceProvider services, EventData data, string partitionId, Guid eventId, CancellationToken cancellationToken)
    {
        var processedEvents = services.GetRequiredService<IProcessedEventRepository>();
        var unitOfWork = services.GetRequiredService<IUnitOfWork>();

        if (await processedEvents.HasProcessedAsync(eventId, cancellationToken))
        {
            _logger.LogInformation("Skip duplicate EventId= {EventId} ", eventId);
            return;
        }

        await _dispatcher.DispatchAsync(data, partitionId, cancellationToken);

        processedEvents.Add(eventId, GetEventProperty(data, "eventType") ?? string.Empty);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private static string? GetEventProperty(EventData data, string key)
    {
        return data.Properties.TryGetValue(key, out var value) ? value?.ToString() : null;
    }

    private Task OnProcessError(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Processor error on partition {PartitionId}", args.PartitionId);
        return Task.CompletedTask;
    }
}
