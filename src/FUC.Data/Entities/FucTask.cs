using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;
public class FucTask : AuditableSoftDeleteEntity 
{
    public Guid Id { get; set; }
    public string KeyTask { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string AssigneeId { get; set; } // studentId
    public string ReporterId { get; set; } // studentId
    public FucTaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime DueDate { get; set; }
    public string? Comment { get; set; }
    public Guid ProjectProgressWeekId { get; set; }

    public Student Reporter { get; set; } = null!;
    public Student Assignee { get; set; } = null!;
    public ProjectProgressWeek ProjectProgressWeek { get; set; }
    public ICollection<FucTaskHistory> FucTaskHistories { get; set; }
}

