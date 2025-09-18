using Domain.Enums;

namespace Domain.Entities;

public class UploadEvent
{
    public Guid Id { get; set; }
    public Guid UploadSessionId { get; set; }
    public UploadSession UploadSession { get; set; } = default!;
    public UploadState State { get; set; }
    public string Message { get; set; } = "";
    public DateTimeOffset At { get; set; } = DateTimeOffset.UtcNow;
}
