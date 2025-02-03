using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;


public sealed class Student : AuditableSoftDeleteEntity
{
    public string Id { get; set; } = string.Empty;
    public string MajorId { get; set; }
    public string CapstoneId { get; set; }
    public string CampusId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public StudentStatus Status { get; set; }

    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public GroupMember GroupMember { get; set; } = null!;
}
