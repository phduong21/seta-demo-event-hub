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
        var response = await _diagnosticsService.TestSendAsync(cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }

    [HttpGet("receive")]
    public async Task<IActionResult> Receive(CancellationToken cancellationToken)
    {
        var response = await _diagnosticsService.TestReceiveAsync(cancellationToken);

        if (response.Success)
        {
            return Ok(response);
        }

        return StatusCode(StatusCodes.Status503ServiceUnavailable, response);
    }
}
