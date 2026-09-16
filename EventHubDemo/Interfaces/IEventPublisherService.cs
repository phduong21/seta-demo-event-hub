using EventHubDemo.Models.Events;
using EventHubDemo.Models.Responses;

namespace EventHubDemo.Interfaces;

public interface IEventPublisherService
{
    Task<ApiResponse<PublishResponse>> PublishAsync<T>(EventEnvelope<T> envelope, string partitionKey, CancellationToken cancellationToken);
}
