using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class FucTaskDetailResponse
{
    public Guid Id { get; set; }
    public string KeyTask { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string AssigneeId { get; set; } 
    public string ReporterId { get; set; }
    public FucTaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? CompletionDate { get; set; }
    public Guid ProjectProgressId { get; set; }

    public string ReporterName { get; set; }
    public string AssigneeName { get; set; }
    public ICollection<FucTaskHistoryDto> FucTaskHistories { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? LastUpdatedDate { get; set; }
}

public class FucTaskHistoryDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Content { get; set; }
    public DateTime CreatedDate { get; set; }
}
