using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.DocumentDTO;
public class UploadTemplateDocumentRequest
{
    public string? ParentId { get; set; } = null!;
    public required IFormFile File { get; set; }
}

public class UploadFolderTemplateDocumentRequest
{
    public string? ParentId { get; set; } = null!;
    public required string FolderName { get; set; }
}
