using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class JoinGroupRequest : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string StudentId { get; set; } // student id want to join group 
    public JoinGroupRequestStatus Status { get; set; }
    public Group Group { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
