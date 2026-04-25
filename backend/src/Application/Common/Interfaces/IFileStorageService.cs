namespace Rawnex.Application.Common.Interfaces;

public interface IFileStorageService
{
    Task<string> UploadAsync(Stream fileStream, string fileName, string containerName, CancellationToken ct = default);
    Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default);
    Task<Stream?> DownloadAsync(string fileUrl, CancellationToken ct = default);
}
