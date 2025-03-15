using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class CreateTaskRequest
{
    public string KeyTask { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public required string AssigneeId { get; set; }
    public required Guid ProjectProgressId { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; } 
}
