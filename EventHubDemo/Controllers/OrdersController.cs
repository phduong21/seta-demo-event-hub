using EventHubDemo.Interfaces;
using EventHubDemo.Models.Events;
using EventHubDemo.Models.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EventHubDemo.Controllers;

[ApiController]
[Route("orders")]
public class OrdersController : ControllerBase
{
    private readonly IEventPublisherService _publisher;

    public OrdersController(IEventPublisherService publisher)
    {
        _publisher = publisher;
    }

    [HttpPost]
    public Task<IActionResult> Create(OrderCreated payload, CancellationToken cancellationToken)
    {
        return PublishAsync(EventTypes.OrderCreated, payload, payload.OrderId, cancellationToken);
    }

    [HttpPost("{orderId:guid}/processing")]
    public Task<IActionResult> Processing(Guid orderId, CancellationToken cancellationToken)
    {
        var payload = new OrderProcessing { OrderId = orderId };
        return PublishAsync(EventTypes.OrderProcessing, payload, orderId, cancellationToken);
    }

    [HttpPost("{orderId:guid}/completed")]
    public Task<IActionResult> Completed(Guid orderId, CancellationToken cancellationToken)
    {
        var payload = new OrderCompleted { OrderId = orderId };
        return PublishAsync(EventTypes.OrderCompleted, payload, orderId, cancellationToken);
    }

    [HttpPost("{orderId:guid}/cancelled")]
    public Task<IActionResult> Cancelled(Guid orderId, CancelOrderRequest request, CancellationToken cancellationToken)
    {
        var payload = new OrderCancelled { OrderId = orderId, Reason = request.Reason };
        return PublishAsync(EventTypes.OrderCancelled, payload, orderId, cancellationToken);
    }

    private async Task<IActionResult> PublishAsync<T>(string eventType, T payload, Guid orderId, CancellationToken cancellationToken)
    {
        var envelope = EventEnvelope<T>.Create(eventType, payload);
        var response = await _publisher.PublishAsync(envelope, orderId.ToString(), cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
