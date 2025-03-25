using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class ProjectProgress : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string MeetingDate { get; set; }
    public string Slot { get; set; }
    public Group Group { get; set; } = null!;
    public ICollection<ProjectProgressWeek> ProjectProgressWeeks { get; set; } = new List<ProjectProgressWeek>();
    public ICollection<FucTask> FucTasks { get; set; } = new List<FucTask>();
}
