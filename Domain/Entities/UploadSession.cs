using Domain.Enums;

namespace Domain.Entities;

public class UploadSession
{
    public Guid Id { get; set; }
    public string UploadId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string OriginalFileName { get; set; } = default!;
    public string ContentType { get; set; } = default!;
    public long TotalSize { get; set; }
    public int ChunkSize { get; set; }
    public int TotalChunks { get; set; }
    public long ReceivedBytes { get; set; }
    public int ReceivedChunks { get; set; }
    public UploadState State { get; set; } = UploadState.Uploading;
    public string TempFolder { get; set; } = default!;
    public string? ReassembledPath { get; set; }
    public string? Sha256 { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    public List<UploadChunk> Chunks { get; set; } = new();
}
