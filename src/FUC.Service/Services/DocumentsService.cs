using Amazon.S3;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DocumentDTO;
using FUC.Service.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class DocumentsService(ILogger<DocumentsService> logger,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<TemplateDocument> templateDocumentRepository,
    IS3Service s3Service,
    ICacheService cacheService,
    S3BucketConfiguration s3BucketConfiguration) : IDocumentsService
{
    public async Task<OperationResult<List<TemplateDocumentRespone>>> GetTemplateDocuments(CancellationToken cancellationToken)
    {
        try
        {
            var cacheKey = s3BucketConfiguration.FUCTemplateBucket;

            var result = await cacheService.GetAsync<List<TemplateDocumentRespone>>(cacheKey, cancellationToken);

            if (result is null)
            {
                var query = from t in templateDocumentRepository.GetQueryable()
                            select new TemplateDocumentRespone
                            {
                                Id = t.Id,
                                FileUrl = t.FileUrl,
                                IsActive = t.IsActive,
                            };

                result = await query.ToListAsync(cancellationToken);

                await cacheService.SetAsync(cacheKey, result, cancellationToken);
            }

            return OperationResult.Success(result);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure<List<TemplateDocumentRespone>>(new Error("Document.Error", ex.Message));
        }
    }

    // View template documents
    public async Task<OperationResult<string>> PresentTemplateDocumentFilePresignedUrl(Guid templateId, CancellationToken cancellationToken)
    {
        var template = await templateDocumentRepository.GetAsync(x => x.Id == templateId, cancellationToken);

        if (template is null)
        {
            return OperationResult.Failure<string>(new Error("Document.Error","Template does not exist."));
        }

        return await PresentFilePresignedUrl(s3BucketConfiguration.FUCTemplateBucket, template.FileUrl);
    }

    private async Task<OperationResult<string>> PresentFilePresignedUrl(string bucketName, string key)
    {
        try
        {
           var cacheKey = string.Join("/", bucketName, key);

            var presignedUrl = await cacheService.GetAsync<string>(cacheKey, default);

            if (presignedUrl is null)
            {
                presignedUrl = await s3Service.GetPresignedUrl(bucketName, key, 1440, isUpload: false);

                await cacheService.SetTimeoutAsync(cacheKey, presignedUrl, TimeSpan.FromDays(1), default);
            }

            return OperationResult.Success(presignedUrl);
        }
        catch (Exception ex)
        {
            return OperationResult.Failure<string>(new Error("Document.Error", ex.Message));
        }
    }

    public async Task<OperationResult> CreateTemplateDocument(string keyPrefix, IFormFile file, CancellationToken cancellationToken)
    {
        try
        {
            await cacheService.RemoveAsync(s3BucketConfiguration.FUCTemplateBucket, cancellationToken);
            
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var key = string.Join('/', keyPrefix, file.FileName);

            var keyCount = await templateDocumentRepository.CountAsync(x => x.FileUrl == key, cancellationToken);

            if (keyCount > 0)
            {
                key += $"({keyCount})"; 
            }

            var activedTemplateDocument = await templateDocumentRepository
                .GetAsync(x => x.FileUrl.StartsWith(keyPrefix) && x.IsActive, 
                isEnabledTracking: true, null, null, 
                cancellationToken);

            if (activedTemplateDocument is not null)
            {
                activedTemplateDocument.IsActive = false;   
            }

            var templateDocument = new TemplateDocument
            {
                FileName = file.FileName,
                FileUrl = key,
                IsActive = true
            };

            templateDocumentRepository.Insert(templateDocument);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            if (!await SaveDocumentToS3(file, s3BucketConfiguration.FUCTemplateBucket, key, cancellationToken))
            {
                throw new AmazonS3Exception("Failed to save to S3, database changes rolled back.");
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception)
        {
            logger.LogError("Fail to upload document template file {FileName}.", file.FileName);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("TemplateDocument.Error", $"Fail to upload file {file.FileName}"));
        }
    }

    public async Task<OperationResult> DeleteTemplateDocument(Guid templateId, CancellationToken cancellationToken)
    {
        try
        {
            await cacheService.RemoveAsync(s3BucketConfiguration.FUCTemplateBucket, cancellationToken);

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var template = await templateDocumentRepository.GetAsync(x => x.Id == templateId, 
                isEnabledTracking: true, null, null, 
                cancellationToken);

            if (template is null)
            {
                return OperationResult.Success();
            }

            if (template.IsActive)
            {
                return OperationResult.Failure(new Error("Document.Error", "Can not delete Active file"));
            }
            
            templateDocumentRepository.Delete(template);

            if (!await RemoveDocumentToS3(s3BucketConfiguration.FUCTemplateBucket, template.FileUrl))
            {
                throw new AmazonS3Exception("Failed to delete to S3, database changes rolled back.");
            }

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception)
        {
            logger.LogError("Fail to delete document template file {FileName}.", templateId);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("TemplateDocument.Error", $"Fail to delete file."));
        }
    }

    public async Task<OperationResult> UpdateActiveStatusForTemplateDocument(Guid templateId, CancellationToken cancellationToken)
    {
        try
        {
            await cacheService.RemoveAsync(s3BucketConfiguration.FUCTemplateBucket, cancellationToken);

            var template = await templateDocumentRepository.GetAsync(x => x.Id == templateId,
            isEnabledTracking: true, null, null,
            cancellationToken);

            if (template is null)
            {
                return OperationResult.Failure(new Error("Document.Error", "Template does not exist."));
            }

            if (template.IsActive)
            {
                return OperationResult.Failure(new Error("Document.Error", "Template is already actived."));
            }

            var prefixKey = string.Join("/", template.FileUrl.Split('/')[..^1]);

            var activedTemplateDocument = await templateDocumentRepository
                    .GetAsync(x => x.FileUrl.StartsWith(prefixKey) && x.IsActive,
                    isEnabledTracking: true, null, null,
                    cancellationToken);

            if (activedTemplateDocument is not null)
            {
                activedTemplateDocument.IsActive = false;
            }

            template.IsActive = true;

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            logger.LogError("Fail to save with error {Messagge}", ex.Message);
            return OperationResult.Failure(new Error("Document.Error", "Fail to change active status."));
        }
    }

    private async Task<bool> SaveDocumentToS3(IFormFile file , string bucketName, string key, CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await s3Service.SaveToS3(bucketName, key, file.ContentType, stream, cancellationToken);

        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    private async Task<bool> RemoveDocumentToS3(string bucketName, string key)
    {
        var result = await s3Service.DeleteFromS3(bucketName, key);

        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}
