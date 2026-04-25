using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _basePath;
    private readonly ILogger<FileStorageService> _logger;

    public FileStorageService(IWebHostEnvironment env, ILogger<FileStorageService> logger)
    {
        _logger = logger;
        _basePath = Path.Combine(env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"), "uploads");
        Directory.CreateDirectory(_basePath);
    }

    public async Task<string> UploadAsync(Stream fileStream, string fileName, string containerName, CancellationToken ct = default)
    {
        var safeContainer = Path.GetFileName(containerName);
        var safeFileName = $"{Guid.NewGuid():N}_{Path.GetFileName(fileName)}";
        var containerPath = Path.Combine(_basePath, safeContainer);
        Directory.CreateDirectory(containerPath);

        var filePath = Path.Combine(containerPath, safeFileName);
        await using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);
        await fileStream.CopyToAsync(fs, ct);

        var relativeUrl = $"/uploads/{safeContainer}/{safeFileName}";
        _logger.LogInformation("File uploaded: {Url}", relativeUrl);
        return relativeUrl;
    }

    public Task<bool> DeleteAsync(string fileUrl, CancellationToken ct = default)
    {
        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        // Ensure the resolved path is under our uploads directory
        var resolvedPath = Path.GetFullPath(fullPath);
        if (!resolvedPath.StartsWith(Path.GetFullPath(_basePath), StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Attempted to delete file outside uploads directory: {Path}", fileUrl);
            return Task.FromResult(false);
        }

        if (File.Exists(resolvedPath))
        {
            File.Delete(resolvedPath);
            _logger.LogInformation("File deleted: {Url}", fileUrl);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<Stream?> DownloadAsync(string fileUrl, CancellationToken ct = default)
    {
        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath);

        var resolvedPath = Path.GetFullPath(fullPath);
        if (!resolvedPath.StartsWith(Path.GetFullPath(_basePath), StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("Attempted to download file outside uploads directory: {Path}", fileUrl);
            return Task.FromResult<Stream?>(null);
        }

        if (!File.Exists(resolvedPath))
            return Task.FromResult<Stream?>(null);

        Stream stream = new FileStream(resolvedPath, FileMode.Open, FileAccess.Read);
        return Task.FromResult<Stream?>(stream);
    }
}
