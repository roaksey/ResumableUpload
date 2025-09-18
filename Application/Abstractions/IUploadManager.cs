using Application.Models;
using Domain.Entities;

namespace Application.Abstractions;

public interface IUploadManager
{
    Task<UploadSession> CreateOrGetSessionAsync(
        string userId, string uploadId, string fileName, string contentType,
        long totalSize, int chunkSize, int totalChunks, CancellationToken ct);

    Task<bool> StoreChunkAsync(string uploadId, int index, long size, Stream content, CancellationToken ct);

    Task<bool> TryReassembleAsync(string uploadId, CancellationToken ct);

    Task<UploadStatusDto> GetStatusAsync(string uploadId, CancellationToken ct);
}
