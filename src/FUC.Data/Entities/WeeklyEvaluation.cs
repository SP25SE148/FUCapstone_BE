using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class WeeklyEvaluation : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string StudentId { get; set; }
    public string SupervisorId { get; set; }
    public Guid ProjectProgressWeekId { get; set; }
    public double ContributionPercentage { get; set; }
    public string Comments { get; set; }
    public EvaluationStatus Status { get; set; }

    public Student Student { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
    public ProjectProgressWeek ProjectProgressWeek { get; set; } = null!;
}
