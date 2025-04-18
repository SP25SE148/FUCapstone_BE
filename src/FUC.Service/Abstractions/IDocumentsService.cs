﻿using FUC.Common.Shared;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.Abstractions;

public interface IDocumentsService
{
    Task<OperationResult<IList<TemplateDocumentRespone>>> GetSubTemplateDocuments(Guid? templateId, CancellationToken cancellationToken);
    Task<OperationResult<string>> PresentTemplateDocumentFilePresignedUrl(Guid templateId, CancellationToken cancellationToken);
    Task<OperationResult> CreateTemplateDocument(Guid? parentId, string? folderName, IFormFile? file, CancellationToken cancellationToken);
    Task<OperationResult> DeleteTemplateDocument(Guid templateId, CancellationToken cancellationToken);
    Task<OperationResult> UpdateActiveStatusForTemplateDocument(Guid templateId, CancellationToken cancellationToken);
    Task<OperationResult> CreateGroupDocument(IFormFile file, string key, CancellationToken cancellationToken);
    Task<OperationResult> CreateThesisDocument(IFormFile file, string key, CancellationToken cancellationToken);
    Task<bool> ThesisDocumentFileExistAsync(string thesisKey);
    Task<bool> GroupDocumentFileExistAsync(string groupKey);
    Task<OperationResult<string>> PresentGroupDocumentFilePresignedUrl(string groupKey);
    Task<OperationResult<string>> PresentEvaluationProjectProgressTemplatePresignedUrl();
    Task<OperationResult<string>> PresentReviewsCalendarsTemplatePresignedUrl();
    Task<OperationResult<string>> PresentDefenseCalendarTemplatePresignedUrl();
    Task<OperationResult<string>> PresentStudentsImportTemplatePresignedUrl();
    Task<OperationResult<string>> PresentSupervisorsImportTemplatePresignedUrl();
    Task<OperationResult<string>> PresentThesisCouncilMeetingMinutesTemplatePresignedUrl();
    Task<OperationResult<string>> PresentTopicRegistrationTemplatePresignedUrl();
    Task<OperationResult<string>> PresentThesisCouncilMeetingMinutesForTopicPresignedUrl(string thesisKey);
    Task<OperationResult<string>> PresentTopicRegistrationFilePresignedUrl(string topicKey);
}
