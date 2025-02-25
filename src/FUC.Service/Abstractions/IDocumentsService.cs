using FUC.Common.Shared;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.Abstractions;

public interface IDocumentsService
{
    Task<OperationResult<List<TemplateDocumentRespone>>> GetTemplateDocuments(CancellationToken cancellationToken);
    Task<OperationResult<string>> PresentTemplateDocumentFilePresignedUrl(Guid templateId, CancellationToken cancellationToken);
    Task<OperationResult> CreateTemplateDocument(string keyPrefix, IFormFile file, CancellationToken cancellationToken);
    Task<OperationResult> DeleteTemplateDocument(Guid templateId, CancellationToken cancellationToken);
    Task<OperationResult> UpdateActiveStatusForTemplateDocument(Guid templateId, CancellationToken cancellationToken);
}
