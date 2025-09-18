using Domain.Enums;

namespace Application.Models;

public class UploadStatusDto
{
    public string UploadId { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public UploadState State { get; set; }
    public long TotalBytes { get; set; }
    public long ReceivedBytes { get; set; }
    public int TotalChunks { get; set; }
    public int ReceivedChunks { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
}
