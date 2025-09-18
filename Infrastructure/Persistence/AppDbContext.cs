using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<UploadSession> UploadSessions => Set<UploadSession>();
    public DbSet<UploadChunk> UploadChunks => Set<UploadChunk>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<UploadSession>(e =>
        {
            e.HasIndex(x => x.UploadId).IsUnique();
            e.Property(x => x.UploadId).HasMaxLength(100);
        });
        b.Entity<UploadChunk>(e =>
        {
            e.HasIndex(x => new { x.UploadSessionId, x.Index }).IsUnique();
        });
    }
}
