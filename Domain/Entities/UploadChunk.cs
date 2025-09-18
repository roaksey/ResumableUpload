namespace Domain.Entities;

public class UploadChunk
{
    public Guid Id { get; set; }
    public Guid UploadSessionId { get; set; }
    public UploadSession UploadSession { get; set; } = default!;
    public int Index { get; set; } // 0-based chunk index
    public long Size { get; set; }
    public bool Stored { get; set; }
    public string Path { get; set; } = default!;
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;
}
