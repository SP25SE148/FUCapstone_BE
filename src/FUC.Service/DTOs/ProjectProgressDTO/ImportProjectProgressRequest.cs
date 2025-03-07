using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class ImportProjectProgressRequest
{
    public IFormFile File {  get; set; }    
    
    public Guid GroupId { get; set; }
}
