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
    public string Email { get; set; }
    public bool IsEligible { get; set; }
    public StudentStatus Status { get; set; }

    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
