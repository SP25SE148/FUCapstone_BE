using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public class DocumentsController(IDocumentsService documentsService) : ApiController
{
    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplateDocuments([FromQuery] Guid? templateId)
    {
        var result = await documentsService.GetSubTemplateDocuments(templateId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("templates/folder")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> CreateFolderTemplateDocument([FromBody] UploadFolderTemplateDocumentRequest request)
    {
        var result = await documentsService.CreateTemplateDocument(string.IsNullOrEmpty(request.ParentId) ? null : 
            Guid.Parse(request.ParentId), 
            request.FolderName, null, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("templates/file")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> CreateTemplateDocument([FromForm] UploadTemplateDocumentRequest request)
    {
        var result = await documentsService.CreateTemplateDocument(string.IsNullOrEmpty(request.ParentId) ? null : 
            Guid.Parse(request.ParentId), null, request.File, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("templates/presigned/{id}")]
    public async Task<IActionResult> GetTemplateDocumentPresignedUrl(string id)
    {
        var presignedUrlResult = await documentsService.PresentTemplateDocumentFilePresignedUrl(Guid.Parse(id), default);

        return presignedUrlResult.IsSuccess ? Ok(presignedUrlResult) : HandleFailure(presignedUrlResult);   
    }

    [HttpDelete("templates/{id}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> DeleteTemplateDocument(string id)
    {
        var result = await documentsService.DeleteTemplateDocument(Guid.Parse(id), cancellationToken: default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("templates/{id}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}")]
    public async Task<IActionResult> UpdateActiveStatusForTemplateDocument(string id)
    {
        var result = await documentsService.UpdateActiveStatusForTemplateDocument(Guid.Parse(id), default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("project-progress")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentEvaluationProjectProgressTemplatePresignedUrl()
    {
        var result = await documentsService.PresentEvaluationProjectProgressTemplatePresignedUrl();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("reviews-calendars")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentReviewsCalendarsTemplatePresignedUrl()
    {
        var result = await documentsService.PresentReviewsCalendarsTemplatePresignedUrl();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("defense-calendar")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentDefenseCalendarTemplatePresignedUrl()
    {
        var result = await documentsService.PresentDefenseCalendarTemplatePresignedUrl();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("students")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentStudentsImportTemplatePresignedUrl()
    {
        var result = await documentsService.PresentStudentsImportTemplatePresignedUrl();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("supervisors")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentSupervisorsImportTemplatePresignedUrl()
    {
        var result = await documentsService.PresentSupervisorsImportTemplatePresignedUrl();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }
}
