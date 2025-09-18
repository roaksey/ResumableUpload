namespace Application.Abstractions;

public interface IChunkStorage
{
    string CreateSessionFolder(string uploadId);
    Task<string> SaveChunkAsync(string sessionFolder, int index, Stream content, CancellationToken ct);
    Task<string> ReassembleAsync(string sessionFolder, int totalChunks, string outputFileName, CancellationToken ct);
}
