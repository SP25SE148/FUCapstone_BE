using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FUC.Service.Abstractions;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Infrastructure;
public class S3Service(
    IAmazonS3 S3Client,
    ILogger<S3Service> logger,
    ITransferUtility transferUtility) : IS3Service
{
    private readonly bool _logTime = bool.TryParse(Environment.GetEnvironmentVariable("ENABLE_LOG_TIME"), out var result) && result;
    private const long PartSize = 5 * 1024 * 1024;

    public async Task<PutObjectResponse> SaveToS3(string bucketName,
        string key,
        string contentType,
        Stream inputStream,
        CancellationToken cancellationToken = default,
        bool disposeStream = false)
    {
        long startTime = 0;
        var s3SaveResponse = new PutObjectResponse()
        {
            HttpStatusCode = HttpStatusCode.OK
        };

        try
        {
            if (_logTime)
            {
                startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            if (inputStream.Length < PartSize)
            {
                var putObjectRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ContentType = contentType,
                    InputStream = inputStream
                };

                s3SaveResponse = await S3Client.PutObjectAsync(putObjectRequest, cancellationToken);
            }
            else
            {
                var transferUtilityUploadRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ContentType = contentType,
                    InputStream = inputStream,
                    AutoResetStreamPosition = false,
                    PartSize = PartSize
                };

                await transferUtility.UploadAsync(transferUtilityUploadRequest, cancellationToken);
            }

            if (disposeStream)
            {
                await inputStream.DisposeAsync();
            }

            return s3SaveResponse;

        }
        catch (AmazonS3Exception ex)
        {
            logger.LogError(ex, "Failed to save file to S3.");
            throw;
        }
        finally
        {
            if (_logTime)
            {
                var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var duration = endTime - startTime;
                logger.LogInformation("SavToS3 took {Duration} ms", duration);
            }
        }
    }

    public async Task<GetObjectResponse?> GetFromS3(string? bucketName, string key)
    {
        long startTime = 0;
        try
        {
            if (_logTime)
            {
                startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            var getObjectRequest = new GetObjectRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var s3Response = await S3Client.GetObjectAsync(getObjectRequest);

            return s3Response;
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
        {
            logger.LogWarning(ex, "The specified key does not exist.");
        }
        finally
        {
            if (_logTime)
            {
                var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var duration = endTime - startTime;
                logger.LogInformation("GetFromS3 took {Duration} ms", duration);
            }
        }

        return default;
    }

    public async Task<DeleteObjectResponse> DeleteFromS3(string bucketName, string key)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var s3Response = await S3Client.DeleteObjectAsync(deleteObjectRequest);

        return s3Response;
    }

    public async Task<string> GetPresignedUrl(string bucketName, string key, short timeToLiveInMinute,
        bool isUpload = true)
    {
        return await S3Client.GetPreSignedURLAsync(
            new GetPreSignedUrlRequest
            {
                BucketName = bucketName,
                Expires = DateTime.UtcNow.AddMinutes(timeToLiveInMinute),
                Key = key,
                Verb = isUpload ? HttpVerb.PUT : HttpVerb.GET
            }
        );
    }

    public async Task<bool> ExistAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };

            var response = await S3Client.GetObjectMetadataAsync(request);
            return true;
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false; // The file does not exist
            }
            else
            {
                throw; // Re-throw the exception if it's not a "Not Found" error
            }
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred: {Message}", e.Message);

            throw;
        }
    }

    public async Task<CopyObjectResponse> CopyObjectAsync(string bucketName, string sourceKey, string destinationKey)
    {
        var copyRequest = new CopyObjectRequest
        {
            SourceBucket = bucketName,
            SourceKey = sourceKey,
            DestinationBucket = bucketName,
            DestinationKey = destinationKey
        };

        return await S3Client.CopyObjectAsync(copyRequest);
    }

    public async Task<bool> MoveFileAsync(string bucketName, List<string> s3SourceKey, string destinationFolder)
    {
        try
        {
            // Store files to be deleted later
            var deleteObjectsRequest = new DeleteObjectsRequest
            {
                BucketName = bucketName,
                Objects = new List<KeyVersion>()
            };
            foreach (var sourceKey in s3SourceKey)
            {
                string destinationKey = destinationFolder + Path.GetFileName(sourceKey);
                // Copy file to the destination folder
                var copyRequest = new CopyObjectRequest
                {
                    SourceBucket = bucketName,
                    SourceKey = sourceKey,
                    DestinationBucket = bucketName,
                    DestinationKey = destinationKey
                };
                await S3Client.CopyObjectAsync(copyRequest);
                // Add the source file to the delete request
                deleteObjectsRequest.Objects.Add(new KeyVersion { Key = sourceKey });
            }

            // Delete all copied files from the source folder
            if (deleteObjectsRequest.Objects.Count > 0)
            {
                await S3Client.DeleteObjectsAsync(deleteObjectsRequest);
            }

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to move files from {S3SourceKey} to {DestinationFolder}", s3SourceKey, destinationFolder);
        }

        return false;
    }

    public async Task<GetObjectMetadataResponse?> GetObjectMetadataAsync(string bucketName, string key)
    {
        try
        {
            var request = new GetObjectMetadataRequest
            {
                BucketName = bucketName,
                Key = key
            };

            return await S3Client.GetObjectMetadataAsync(request);
        }
        catch (AmazonS3Exception e)
        {
            if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return default;
            }

            throw;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Cannot get metadata for {Key} from {BucketName}", key, bucketName);

            throw;
        }
    }

    public async Task<List<string>> GetS3ObjectKeys(string bucketName, string? prefix)
    {
        long startTime = 0;
        var s3ObjectKeys = new List<string>();
        try
        {
            if (_logTime)
            {
                startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            var listObjectsV2Request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = prefix,
                Delimiter = "/"
            };

            var s3Response = await S3Client.ListObjectsV2Async(listObjectsV2Request);

            foreach (var s3 in s3Response.S3Objects)
            {
                s3ObjectKeys.Add(s3.Key);
            }
        }
        catch (AmazonS3Exception ex) when (ex.Message is "The specified key does not exist.")
        {
            logger.LogWarning(ex, "The specified key does not exist.");
        }
        finally
        {
            if (_logTime)
            {
                var endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                var duration = endTime - startTime;
                logger.LogInformation("GetFromS3 took {Duration} ms", duration);
            }
        }

        return s3ObjectKeys;
    }
}
