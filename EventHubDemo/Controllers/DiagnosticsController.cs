using EventHubDemo.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EventHubDemo.Controllers;

[ApiController]
[Route("diagnostics")]
public class DiagnosticsController : ControllerBase
{
    private readonly IEventHubDiagnosticsService _diagnosticsService;

    public DiagnosticsController(IEventHubDiagnosticsService diagnosticsService)
    {
        _diagnosticsService = diagnosticsService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> Send(CancellationToken cancellationToken)
    {
        var result = await _diagnosticsService.TestSendAsync(cancellationToken);
        return result.Ok
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }

    [HttpGet("receive")]
    public async Task<IActionResult> Receive(CancellationToken cancellationToken)
    {
        var result = await _diagnosticsService.TestReceiveAsync(cancellationToken);
        return result.Ok
            ? Ok(result)
            : StatusCode(StatusCodes.Status503ServiceUnavailable, result);
    }
}
