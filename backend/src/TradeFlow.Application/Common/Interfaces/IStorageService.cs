namespace TradeFlow.Application.Common.Interfaces;

public interface IStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default);
    Task<string> GenerateSignedDownloadUrlAsync(string fileKey, TimeSpan expiry, CancellationToken ct = default);
    Task DeleteFileAsync(string fileKey, CancellationToken ct = default);
}
