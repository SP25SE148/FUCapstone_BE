using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class ProjectProgress : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string SupervisorId  { get; set; }
    public string MeetingDate { get; set; }
    public Group Group { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
    public ICollection<ProjectProgressWeek> ProjectProgressWeeks { get; set; } = new List<ProjectProgressWeek>();
}
