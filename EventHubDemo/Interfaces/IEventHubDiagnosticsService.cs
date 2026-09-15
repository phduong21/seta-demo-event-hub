using EventHubDemo.Models;

namespace EventHubDemo.Interfaces;

public interface IEventHubDiagnosticsService
{
    Task<SendResult> TestSendAsync(CancellationToken cancellationToken);

    Task<ReceiveResult> TestReceiveAsync(CancellationToken cancellationToken);
}
