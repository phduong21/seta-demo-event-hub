using EventHubDemo.Models.Enums;
using EventHubDemo.Models.Events;

namespace EventHubDemo.Tests;

public class EventEnvelopeTests
{
    [Fact]
    public void CreateFillsEnvelope()
    {
        var payload = new OrderCreated
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Amount = 10,
            Currency = Currency.USD
        };

        var envelope = EventEnvelope<OrderCreated>.Create(EventTypes.OrderCreated, payload);

        Assert.NotEqual(Guid.Empty, envelope.EventId);
        Assert.Equal(EventTypes.OrderCreated, envelope.EventType);
        Assert.Same(payload, envelope.Payload);
    }

    [Fact]
    public void CreateGivesNewIdEachTime()
    {
        var payload = new OrderProcessing { OrderId = Guid.NewGuid() };

        var first = EventEnvelope<OrderProcessing>.Create(EventTypes.OrderProcessing, payload);
        var second = EventEnvelope<OrderProcessing>.Create(EventTypes.OrderProcessing, payload);

        Assert.NotEqual(first.EventId, second.EventId);
    }
}
