namespace FUC.Service.DTOs.DocumentDTO;

public class TemplateDocumentRespone
{
    public Guid Id { get; set; }
    public string FileUrl { get; set; }
    public string FileName { get; set; }
    public bool IsActive { get; set; }
    public bool IsFile { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}


