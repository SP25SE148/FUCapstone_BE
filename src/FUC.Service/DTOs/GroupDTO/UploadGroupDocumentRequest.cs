using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.GroupDTO;

public class UploadGroupDocumentRequest
{
    public Guid GroupId { get; set; }
    public required IFormFile File { get; set; }
}
