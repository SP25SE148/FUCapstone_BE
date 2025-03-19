using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;
public class FucTask : AuditableSoftDeleteEntity 
{
    public Guid Id { get; set; }
    public string KeyTask { get; set; }
    public string Description { get; set; }
    public string Summary { get; set; }
    public string AssigneeId { get; set; } // who to do task
    public string ReporterId { get; set; } // who assign task for member
    public FucTaskStatus Status { get; set; }
    public Priority Priority { get; set; }
    public DateTime? CompletionDate { get; set; }
    public DateTime DueDate { get; set; }
    public Guid ProjectProgressId { get; set; }

    public Student Reporter { get; set; } = null!;
    public Student Assignee { get; set; } = null!;
    public ProjectProgress ProjectProgress { get; set; } = null!;
    public ICollection<FucTaskHistory> FucTaskHistories { get; set; } = new List<FucTaskHistory>();
}

