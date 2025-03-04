using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class Supervisor : AuditableSoftDeleteEntity
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; }
    public string MajorId { get; set; }
    public string CampusId { get; set; }
    public int MaxGroupsInSemester { get; set; }
    public string Email { get; set; } = string.Empty;
    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public bool IsAvailable { get; set; }

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<CoSupervisor> CoSupervisors { get; set; } = new List<CoSupervisor>();

    public ICollection<TopicAppraisal> TopicAppraisals { get; set; } = new List<TopicAppraisal>();
    public ICollection<TopicRequest> TopicRequests { get; set; } = new List<TopicRequest>();
}
