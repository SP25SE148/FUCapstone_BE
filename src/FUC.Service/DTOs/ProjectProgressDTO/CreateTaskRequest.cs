using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class CreateTaskRequest
{
    public Guid Id { get; set; }
    public string KeyTask { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string AssigneeId { get; set; } 
    public Guid ProjectProgressWeekId { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; } 
}
