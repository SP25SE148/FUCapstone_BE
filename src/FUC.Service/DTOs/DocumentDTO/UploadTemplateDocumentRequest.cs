using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.DocumentDTO;
public class UploadTemplateDocumentRequest
{
    public string Path { get; set; }
    public IFormFile File { get; set; }
}
