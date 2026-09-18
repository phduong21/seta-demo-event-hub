namespace EventHubDemo.Interfaces;

public interface IProcessedEventRepository
{
    Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken);

    void Add(Guid eventId, string eventType);
}
