using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class FucTaskResponse
{
    public Guid Id { get; set; }
    public string KeyTask { get; set; }
    public string Summary { get; set; }
    public string Description { get; set; }
    public string AssigneeId { get; set; } 
    public string ReporterId { get; set; }
    public string? Comment { get; set; }
    public FucTaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
