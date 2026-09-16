using EventHubDemo.Models.Responses;

namespace EventHubDemo.Interfaces;

public interface IEventHubDiagnosticsService
{
    Task<ApiResponse<TestSendResponse>> TestSendAsync(CancellationToken cancellationToken);

    Task<ApiResponse<TestReceiveResponse>> TestReceiveAsync(CancellationToken cancellationToken);
}
