using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using TradeFlow.Application.Common.Interfaces;

namespace TradeFlow.Infrastructure.Services;

public class R2StorageService(IConfiguration config) : IStorageService
{
    private readonly string _bucketName = config["R2:BucketName"] ?? "tradeflow-files";

    private AmazonS3Client CreateClient()
    {
        var accountId = config["R2:AccountId"]!;
        return new AmazonS3Client(
            config["R2:AccessKeyId"],
            config["R2:SecretAccessKey"],
            new AmazonS3Config
            {
                ServiceURL = $"https://{accountId}.r2.cloudflarestorage.com",
                ForcePathStyle = true
            });
    }

    public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType, CancellationToken ct = default)
    {
        using var client = CreateClient();
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = fileName,
            InputStream = fileStream,
            ContentType = contentType
        };
        await client.PutObjectAsync(request, ct);
        return fileName;
    }

    public Task<string> GenerateSignedDownloadUrlAsync(string fileKey, TimeSpan expiry, CancellationToken ct = default)
    {
        using var client = CreateClient();
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = fileKey,
            Expires = DateTime.UtcNow.Add(expiry),
            Verb = HttpVerb.GET
        };
        return Task.FromResult(client.GetPreSignedURL(request));
    }

    public async Task DeleteFileAsync(string fileKey, CancellationToken ct = default)
    {
        using var client = CreateClient();
        await client.DeleteObjectAsync(_bucketName, fileKey, ct);
    }
}
