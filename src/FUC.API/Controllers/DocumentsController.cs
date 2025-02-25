using FUC.API.Abstractions;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public class DocumentsController(IDocumentsService documentsService) : ApiController
{
    [HttpGet("templates")]
    public async Task<IActionResult> GetTemplateDocuments()
    {
        var result = await documentsService.GetTemplateDocuments(default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);   
    }

    [HttpPost("templates")]
    public async Task<IActionResult> UploadTemplateDocument([FromForm] UploadTemplateDocumentRequest request)
    {
        var result = await documentsService.CreateTemplateDocument(request.Path, request.File, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("template/presigned/{id}")]
    public async Task<IActionResult> GetTemplateDocumentPresignedUrl(string id)
    {
        var presignedUrlResult = await documentsService.PresentTemplateDocumentFilePresignedUrl(Guid.Parse(id), default);

        return presignedUrlResult.IsSuccess ? Ok(presignedUrlResult) : HandleFailure(presignedUrlResult);   
    }

    [HttpDelete("template/{id}")]
    public async Task<IActionResult> DeleteTemplateDocument(string id)
    {
        var result = await documentsService.DeleteTemplateDocument(Guid.Parse(id), cancellationToken: default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("template/{id}")]
    public async Task<IActionResult> UpdateActiveStatusForTemplateDocument(string id)
    {
        var result = await documentsService.UpdateActiveStatusForTemplateDocument(Guid.Parse(id), default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }
}
