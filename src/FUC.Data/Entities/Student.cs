using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class Student : AuditableSoftDeleteEntity
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string MajorId { get; set; }
    public string CapstoneId { get; set; }
    public string CampusId { get; set; }
    public Guid? BusinessAreaId { get; set; }
    public string Email { get; set; }
    public float GPA { get; set; }
    public bool IsEligible { get; set; }
    public StudentStatus Status { get; set; }

    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
    public BusinessArea BusinessArea { get; set; } = null!;
    public ICollection<FucTask> FucTasks { get; set; } = new List<FucTask>();
    public ICollection<FucTask> ReportFucTasks { get; set; } = new List<FucTask>();
    public ICollection<WeeklyEvaluation> WeeklyEvaluations { get; set; } = new List<WeeklyEvaluation>();
}
