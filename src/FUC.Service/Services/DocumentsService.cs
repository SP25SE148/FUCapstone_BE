using System.Text.RegularExpressions;
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

public class DocumentsService(
    ILogger<DocumentsService> logger,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<TemplateDocument> templateDocumentRepository,
    IS3Service s3Service,
    ICacheService cacheService,
    S3BucketConfiguration s3BucketConfiguration) : IDocumentsService
{
    private const long MaxZipFileSize = 50 * 1024 * 1024; // 50MB

    public async Task<OperationResult<IList<TemplateDocumentRespone>>> GetSubTemplateDocuments(Guid? templateId,
        CancellationToken cancellationToken)
    {
        // get the root of folder
        if (templateId == null)
        {
            var query = from t in templateDocumentRepository.GetQueryable()
                where t.ParentId == null
                select new TemplateDocumentRespone
                {
                    Id = t.Id,
                    FileUrl = t.FileUrl,
                    FileName = t.FileName,
                    IsActive = t.IsActive,
                    CreatedBy = t.CreatedBy,
                    CreatedDate = t.CreatedDate,
                    IsFile = t.IsFile,
                };

            return await query.ToListAsync(cancellationToken);
        }

        // the subfolder or file of current folder
        var template = await templateDocumentRepository.GetAsync(
            x => x.Id == templateId,
            cancellationToken);

        if (template == null)
        {
            return OperationResult.Failure<IList<TemplateDocumentRespone>>(new Error("Document.Error",
                "Template does not exist."));
        }

        if (template.IsFile)
        {
            return OperationResult.Failure<IList<TemplateDocumentRespone>>(new Error("Document.Error",
                "This is file you can not get subfolder from there."));
        }

        var subfolder = await templateDocumentRepository.FindAsync(
            x => x.ParentId == templateId,
            include: null,
            orderBy: x => x.OrderByDescending(x => !x.IsFile)
                .ThenBy(x => x.FileName),
            selector: t => new TemplateDocumentRespone
            {
                Id = t.Id,
                FileUrl = t.FileUrl,
                FileName = t.FileName,
                IsActive = t.IsActive,
                CreatedBy = t.CreatedBy,
                CreatedDate = t.CreatedDate,
                IsFile = t.IsFile,
            }, cancellationToken);

        return OperationResult.Success(subfolder);
    }

    public async Task<OperationResult> CreateTemplateDocument(Guid? parentId, string? folderName, IFormFile? file,
        CancellationToken cancellationToken)
    {
        try
        {
            var parentTemplate = parentId != null
                ? await templateDocumentRepository.GetAsync(
                    x => x.Id == parentId,
                    cancellationToken)
                : null;

            if (parentId != null && parentTemplate == null)
            {
                return OperationResult.Failure<IList<TemplateDocumentRespone>>(new Error("Document.Error",
                    "Template does not exist."));
            }

            if (parentTemplate != null && parentTemplate.IsFile)
            {
                return OperationResult.Failure<IList<TemplateDocumentRespone>>(new Error("Document.Error",
                    "This is file you can not get subfolder from there."));
            }

            if (file is null)
            {
                if (folderName is null)
                {
                    return OperationResult.Failure<IList<TemplateDocumentRespone>>(new Error("Document.Error",
                        "You can not create subfolder"));
                }

                templateDocumentRepository.Insert(new TemplateDocument
                {
                    FileName = folderName,
                    FileUrl = parentTemplate is not null
                        ? string.Join("/", parentTemplate.FileUrl, folderName)
                        : folderName,
                    ParentId = parentId,
                    IsFile = false,
                    IsActive = true
                });

                await unitOfWork.SaveChangesAsync(cancellationToken);

                return OperationResult.Success();
            }

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var key = parentTemplate is not null
                ? string.Join('/', parentTemplate.FileUrl, file.FileName)
                : file.FileName;

            var fileUrl = await GenerateFileName(key, cancellationToken);

            var activedTemplateDocument = await templateDocumentRepository
                .GetAsync(x => x.FileUrl.StartsWith(parentTemplate != null ? parentTemplate.FileUrl : "") && x.IsActive,
                    isEnabledTracking: true, null, null,
                    cancellationToken);

            if (activedTemplateDocument is not null)
            {
                activedTemplateDocument.IsActive = false;
            }

            var templateDocument = new TemplateDocument
            {
                FileName = fileUrl.Split("/")[^1],
                FileUrl = fileUrl,
                IsActive = true,
                IsFile = true,
                ParentId = parentId
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
        catch (Exception ex)
        {
            logger.LogError("Fail to create subfolder of {ParentId} with error {Message}", parentId, ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("TemplateDocument.Error", $"Fail to create folder/file"));
        }
    }

    // View template documents
    public async Task<OperationResult<string>> PresentTemplateDocumentFilePresignedUrl(Guid templateId,
        CancellationToken cancellationToken)
    {
        var template = await templateDocumentRepository.GetAsync(x => x.Id == templateId, cancellationToken);

        if (template is null)
        {
            return OperationResult.Failure<string>(new Error("Document.Error", "Template does not exist."));
        }

        return !template.IsFile
            ? OperationResult.Failure<string>(new Error("Document.Error", "This is folder you can not do action."))
            : await PresentFilePresignedUrl(s3BucketConfiguration.FUCTemplateBucket, template.FileUrl);
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

    public async Task<OperationResult> DeleteTemplateDocument(Guid templateId, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            var template = await templateDocumentRepository.GetAsync(
                x => x.Id == templateId,
                isEnabledTracking: true, null, null,
                cancellationToken);

            if (template is null)
            {
                return OperationResult.Success();
            }

            if (!template.IsFile)
            {
                if (await templateDocumentRepository.AnyAsync(
                        x => x.ParentId == template.Id,
                        cancellationToken))
                {
                    return OperationResult.Failure(new Error("TemplateDocument.Error", $"Fail to delete folder."));
                }

                templateDocumentRepository.Delete(template);

                await unitOfWork.SaveChangesAsync(cancellationToken);
                await unitOfWork.CommitAsync(cancellationToken);

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

    public async Task<OperationResult> UpdateActiveStatusForTemplateDocument(Guid templateId,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await templateDocumentRepository.GetAsync(
                x => x.Id == templateId,
                isEnabledTracking: true, null, null,
                cancellationToken);

            if (template is null)
            {
                return OperationResult.Failure(new Error("Document.Error", "Template does not exist."));
            }

            if (!template.IsFile)
            {
                return OperationResult.Failure(new Error("Document.Error", "Only update status for file."));
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
        catch (Exception ex)
        {
            logger.LogError("Fail to save with error {Messagge}", ex.Message);
            return OperationResult.Failure(new Error("Document.Error", "Fail to change active status."));
        }
    }

    private async Task<bool> SaveDocumentToS3(IFormFile file, string bucketName, string key,
        CancellationToken cancellationToken)
    {
        using var stream = file.OpenReadStream();

        var result = await s3Service.SaveToS3(bucketName, key, file.ContentType, stream, cancellationToken);

        return result.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }

    private async Task<bool> RemoveDocumentToS3(string bucketName, string key)
    {
        var result = await s3Service.DeleteFromS3(bucketName, key);

        return result.HttpStatusCode == System.Net.HttpStatusCode.OK ||
               result.HttpStatusCode == System.Net.HttpStatusCode.NoContent;
    }

    private async Task<string> GenerateFileName(string newFileUrl, CancellationToken cancellationToken)
    {
        var oldFileUrls = (await templateDocumentRepository.FindAsync(
                x => x.FileUrl.StartsWith(newFileUrl), cancellationToken))
            .Select(f => f.FileUrl).ToList();

        if (oldFileUrls == null || oldFileUrls.Count == 0)
        {
            return newFileUrl;
        }

        if (!oldFileUrls.Contains(newFileUrl))
        {
            return newFileUrl;
        }

        string pattern = $@"^{Regex.Escape(newFileUrl)}(?: \((\d+)\))?$";

        var matches = oldFileUrls
            .Select(f => Regex.Match(f, pattern))
            .Where(m => m.Success)
            .Select(m => m.Groups[1].Success ? int.Parse(m.Groups[1].Value) : 0)
            .ToList();

        int nextIndex = matches.Count > 0 ? matches.Max() + 1 : 1;

        return $"{newFileUrl} ({nextIndex})";
    }

    public async Task<OperationResult> CreateGroupDocument(IFormFile file, string key, CancellationToken cancellationToken)
    {
        if (!IsValidFile(file))
            return OperationResult.Failure(new Error("Document.Error", "File is null."));

        if (file.Length > MaxZipFileSize)
            return OperationResult.Failure(new Error("Document.Error", "File size exceeds the 50MB limit."));

        if (!await IsZipFileAsync(file))
            return OperationResult.Failure(new Error("Document.Error", "File size exceeds the 50MB limit."));

        var result = await SaveDocumentToS3(file, s3BucketConfiguration.FUCGroupDocumentBucket, key, cancellationToken);

        return result ? OperationResult.Success() : OperationResult.Failure(new Error("Document.Error", "Fail to upload documents."));
    }

    public async Task<OperationResult<string>> PresentDocumentFilePresignedUrl(string groupKey)
    {
        var result = await PresentFilePresignedUrl(s3BucketConfiguration.FUCGroupDocumentBucket, groupKey);

        return result.IsFailure
            ? OperationResult.Failure<string>(new Error("Document.Error", "This is folder you can not do action."))
            : result.Value;
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0;
    }

    private async Task<bool> IsZipFileAsync(IFormFile file)
    {
        // Check MIME type (less reliable but useful as a first check)
        var allowedMimeTypes = new[] { "application/zip", "application/x-zip-compressed" };
        if (!allowedMimeTypes.Contains(file.ContentType.ToLower()))
            return false;

        // Read first 4 bytes in a more efficient way
        byte[] header = new byte[4];
        using var stream = file.OpenReadStream();

        int bytesRead = await stream.ReadAsync(header.AsMemory(0, 4));
        if (bytesRead < 4)
            return false; // Invalid ZIP file

        // Reset stream position so the same stream can be reused for uploading
        stream.Position = 0;

        return header.SequenceEqual(new byte[] { 0x50, 0x4B, 0x03, 0x04 }); // ZIP signature
    }

}
