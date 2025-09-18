using Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/upload")]
public class UploadController(IUploadManager manager) : ControllerBase
{
    [HttpPost("init")]
    public async Task<IActionResult> Init(
        [FromHeader(Name = "X-Upload-Id")] string uploadId,
        [FromHeader(Name = "X-File-Name")] string fileName,
        [FromHeader(Name = "X-File-Size")] long totalSize,
        [FromHeader(Name = "X-Chunk-Size")] int chunkSize,
        [FromHeader(Name = "X-Total-Chunks")] int totalChunks,
        [FromHeader(Name = "X-Content-Type")] string contentType,
        CancellationToken ct)
    {
        var userId = (string)HttpContext.Items["UserId"]!;
        var session = await manager.CreateOrGetSessionAsync(userId, uploadId, fileName, contentType, totalSize, chunkSize, totalChunks, ct);
        return Ok(new { session.UploadId, session.State, session.ReceivedChunks, session.TotalChunks });
    }

    [HttpPut("chunk")]
    public async Task<IActionResult> UploadChunk(
        [FromHeader(Name = "X-Upload-Id")] string uploadId,
        [FromHeader(Name = "X-Chunk-Index")] int index,
        CancellationToken ct)
    {
        var ok = await manager.StoreChunkAsync(uploadId, index, Request.ContentLength ?? 0, Request.Body, ct);
        return ok ? Accepted() : StatusCode(StatusCodes.Status409Conflict);
    }

    [HttpPost("finalize")]
    public async Task<IActionResult> Finalize([FromHeader(Name = "X-Upload-Id")] string uploadId, CancellationToken ct)
    {
        var ok = await manager.TryReassembleAsync(uploadId, ct);
        return ok ? Accepted() : BadRequest("Not all chunks received");
    }
}
