using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;


public sealed class GroupMember : Entity
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string StudentId { get; set; } = string.Empty;

    public bool IsLeader { get; set; }
    public GroupMemberStatus Status { get; set; }

    public Group Group { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
