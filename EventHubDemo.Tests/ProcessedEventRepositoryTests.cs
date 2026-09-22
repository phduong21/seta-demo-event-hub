using EventHubDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHubDemo.Tests;

public class ProcessedEventRepositoryTests
{
    private static AppDbContext NewContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task NewEventIsNotProcessed()
    {
        using var context = NewContext();
        var repo = new ProcessedEventRepository(context);

        var processed = await repo.HasProcessedAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.False(processed);
    }

    [Fact]
    public async Task StoredEventIsProcessed()
    {
        using var context = NewContext();
        var repo = new ProcessedEventRepository(context);
        var eventId = Guid.NewGuid();

        repo.Add(eventId, "OrderCreated");
        await context.SaveChangesAsync();

        var processed = await repo.HasProcessedAsync(eventId, CancellationToken.None);

        Assert.True(processed);
    }

    [Fact]
    public async Task AddSavesEvent()
    {
        using var context = NewContext();
        var repo = new ProcessedEventRepository(context);
        var eventId = Guid.NewGuid();

        repo.Add(eventId, "OrderCompleted");
        await context.SaveChangesAsync();

        var saved = await context.ProcessedEvents.SingleAsync();
        Assert.Equal(eventId, saved.EventId);
        Assert.Equal("OrderCompleted", saved.EventType);
    }
}
