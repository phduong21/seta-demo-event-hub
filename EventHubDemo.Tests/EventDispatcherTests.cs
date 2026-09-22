using Azure.Messaging.EventHubs;
using EventHubDemo.Models.Events;
using EventHubDemo.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace EventHubDemo.Tests;

public class EventDispatcherTests
{
    [Fact]
    public async Task KnownEventDoesNotThrow()
    {
        var dispatcher = new EventDispatcher(NullLogger<EventDispatcher>.Instance);
        var data = new EventData("{\"Payload\":{\"OrderId\":\"1\"}}");
        data.Properties["eventType"] = EventTypes.OrderCreated;
        data.Properties["eventId"] = Guid.NewGuid().ToString();

        await dispatcher.DispatchAsync(data, "0", CancellationToken.None);
    }

    [Fact]
    public async Task UnknownEventDoesNotThrow()
    {
        var dispatcher = new EventDispatcher(NullLogger<EventDispatcher>.Instance);
        var data = new EventData("{}");
        data.Properties["eventType"] = "Something";

        await dispatcher.DispatchAsync(data, "0", CancellationToken.None);
    }
}
