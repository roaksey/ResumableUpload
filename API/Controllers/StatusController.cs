using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/status")]
public class StatusController(IUploadManager manager) : ControllerBase
{
    [HttpGet("{uploadId}")]
    public async Task<IActionResult> Get(string uploadId, CancellationToken ct)
    {
        var dto = await manager.GetStatusAsync(uploadId, ct);
        return Ok(dto);
    }
}
