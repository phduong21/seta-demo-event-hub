using EventHubDemo.Interfaces;
using EventHubDemo.Models.Enums;
using EventHubDemo.Models.Events;
using Microsoft.AspNetCore.Mvc;

namespace EventHubDemo.Controllers;

[ApiController]
[Route("dev")]
public class DevController : ControllerBase
{
    private readonly IEventPublisherService _publisher;
    private readonly IWebHostEnvironment _environment;

    public DevController(IEventPublisherService publisher, IWebHostEnvironment environment)
    {
        _publisher = publisher;
        _environment = environment;
    }

    [HttpPost("publish/{eventId:guid}")]
    public async Task<IActionResult> Publish(Guid eventId, CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var payload = new OrderCreated
        {
            OrderId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(),
            Amount = 1,
            Currency = Currency.USD
        };

        var envelope = new EventEnvelope<OrderCreated>
        {
            EventId = eventId,
            EventType = EventTypes.OrderCreated,
            OccurredAt = DateTimeOffset.UtcNow,
            Payload = payload
        };

        var response = await _publisher.PublishAsync(envelope, eventId.ToString(), cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
