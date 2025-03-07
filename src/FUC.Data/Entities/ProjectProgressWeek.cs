using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class ProjectProgressWeek : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid ProjectProgressId { get; set; }
    public int WeekNumber { get; set; }
    public string TaskDescription { get; set; }
    public ProjectProgressWeekStatus Status { get; set; }
    public string? MeetingLocation { get; set; }
    public string? MeetingContent { get; set; }

    public ProjectProgress ProjectProgress { get; set; } = null!;
    public ICollection<FucTask> FucTasks { get; set; } = new List<FucTask>();
    public ICollection<WeeklyEvaluation> WeeklyEvaluations { get; set; } = new List<WeeklyEvaluation>();
}
