using EventHubDemo.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EventHubDemo.Data;

public class ProcessedEventRepository : IProcessedEventRepository
{
    private readonly AppDbContext _db;

    public ProcessedEventRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<bool> HasProcessedAsync(Guid eventId, CancellationToken cancellationToken)
    {
        return _db.ProcessedEvents.AnyAsync(e => e.EventId == eventId, cancellationToken);
    }

    public void Add(Guid eventId, string eventType)
    {
        _db.ProcessedEvents.Add(new ProcessedEvent
        {
            EventId = eventId,
            EventType = eventType,
            ProcessedAt = DateTimeOffset.UtcNow
        });
    }
}
