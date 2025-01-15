using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;
using FUC.Data.Enums;

namespace FUC.Data.Data;

public sealed class Student : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = string.Empty;
    public Guid MajorId { get; set; }
    public Guid CapstoneId { get; set; }
    public Guid CampusId { get; set; }
    public string Email { get; set; } = string.Empty;
    public bool IsEligible { get; set; }
    public bool IsLeader { get; set; }
    public StudentStatus Status { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public GroupMember GroupMember { get; set; } = null!;
}
