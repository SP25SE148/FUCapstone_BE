using Amazon.S3.Model;

namespace FUC.Service.Abstractions;

public interface IS3Service
{
    Task<PutObjectResponse> SaveToS3(string bucketName, string key, string contentType, Stream inputStream,
        CancellationToken cancellationToken = default, bool disposeStream = false);

    Task<GetObjectResponse?> GetFromS3(string? bucketName, string key);

    Task<DeleteObjectResponse> DeleteFromS3(string bucketName, string key);

    Task<string> GetPresignedUrl(string bucketName, string key, short timeToLiveInMinute, bool isUpload = true);

    Task<bool> ExistAsync(string bucketName, string key);

    Task<CopyObjectResponse> CopyObjectAsync(string bucketName, string sourceKey, string destinationKey);

    Task<bool> MoveFileAsync(string bucketName, List<string> s3SourceKey, string destinationFolder);

    Task<GetObjectMetadataResponse?> GetObjectMetadataAsync(string bucketName, string key);
}
