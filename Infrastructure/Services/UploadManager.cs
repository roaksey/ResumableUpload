using Application.Abstractions;
using Application.Models;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Application.Services;

public class UploadManager : IUploadManager
{
    private readonly AppDbContext _db;
    private readonly IChunkStorage _storage;

    public UploadManager(AppDbContext db, IChunkStorage storage)
    {
        _db = db;
        _storage = storage;
    }

    public async Task<UploadSession> CreateOrGetSessionAsync(
        string userId, string uploadId, string fileName, string contentType,
        long totalSize, int chunkSize, int totalChunks, CancellationToken ct)
    {
        var session = await _db.UploadSessions.FirstOrDefaultAsync(x => x.UploadId == uploadId, ct);
        if (session is not null) return session;

        var folder = _storage.CreateSessionFolder(uploadId);

        session = new UploadSession
        {
            UploadId = uploadId,
            UserId = userId,
            OriginalFileName = fileName,
            ContentType = contentType,
            TotalSize = totalSize,
            ChunkSize = chunkSize,
            TotalChunks = totalChunks,
            TempFolder = folder
        };

        _db.UploadSessions.Add(session);
        await _db.SaveChangesAsync(ct);

        return session;
    }

    public async Task<bool> StoreChunkAsync(string uploadId, int index, long size, Stream content, CancellationToken ct)
    {
        var session = await _db.UploadSessions.Include(x => x.Chunks)
            .FirstOrDefaultAsync(x => x.UploadId == uploadId, ct)
            ?? throw new InvalidOperationException("Session not found");

        var path = await _storage.SaveChunkAsync(session.TempFolder, index, content, ct);

        if (!session.Chunks.Any(c => c.Index == index))
        {
            session.Chunks.Add(new UploadChunk
            {
                UploadSessionId = session.Id,
                Index = index,
                Size = size,
                Stored = true,
                Path = path
            });
        }

        session.ReceivedChunks = session.Chunks.Count;
        session.ReceivedBytes = session.Chunks.Sum(c => c.Size);
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> TryReassembleAsync(string uploadId, CancellationToken ct)
    {
        var session = await _db.UploadSessions.Include(x => x.Chunks)
            .FirstAsync(x => x.UploadId == uploadId, ct);

        if (session.ReceivedChunks != session.TotalChunks) return false;

        var outName = Path.GetFileName(session.OriginalFileName);
        var outPath = await _storage.ReassembleAsync(session.TempFolder, session.TotalChunks, outName, ct);

        session.ReassembledPath = outPath;
        session.State = UploadState.Uploaded;
        session.UpdatedAt = DateTimeOffset.UtcNow;

        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<UploadStatusDto> GetStatusAsync(string uploadId, CancellationToken ct)
    {
        var s = await _db.UploadSessions.AsNoTracking().FirstAsync(x => x.UploadId == uploadId, ct);
        return new UploadStatusDto
        {
            UploadId = s.UploadId,
            UserId = s.UserId,
            State = s.State,
            TotalBytes = s.TotalSize,
            ReceivedBytes = s.ReceivedBytes,
            TotalChunks = s.TotalChunks,
            ReceivedChunks = s.ReceivedChunks,
            UpdatedAt = s.UpdatedAt
        };
    }
}
