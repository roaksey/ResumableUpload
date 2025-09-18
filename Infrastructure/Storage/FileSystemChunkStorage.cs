using Application.Abstractions;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Storage;

public class FileSystemChunkStorage : IChunkStorage
{
    private readonly string _root;

    public FileSystemChunkStorage(IConfiguration cfg)
    {
        _root = cfg["Storage:Root"] ?? Path.Combine(Path.GetTempPath(), "resumable-upload");
        Directory.CreateDirectory(_root);
    }

    public string CreateSessionFolder(string uploadId)
    {
        var path = Path.Combine(_root, uploadId);
        Directory.CreateDirectory(path);
        return path;
    }

    public async Task<string> SaveChunkAsync(string sessionFolder, int index, Stream content, CancellationToken ct)
    {
        var path = Path.Combine(sessionFolder, $"{index:D8}.part");
        using var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
        await content.CopyToAsync(fs, ct);
        return path;
    }

    public async Task<string> ReassembleAsync(string sessionFolder, int totalChunks, string outputFileName, CancellationToken ct)
    {
        var outPath = Path.Combine(sessionFolder, outputFileName);
        await using var output = new FileStream(outPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);

        for (var i = 0; i < totalChunks; i++)
        {
            var chunk = Path.Combine(sessionFolder, $"{i:D8}.part");
            await using var input = new FileStream(chunk, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true);
            await input.CopyToAsync(output, ct);
        }

        return outPath;
    }
}
