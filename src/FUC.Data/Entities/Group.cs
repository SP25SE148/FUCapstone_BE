using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;


public sealed class Group : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid SemesterId { get; set; }
    public Guid MajorId { get; set; }
    public Guid CampusId { get; set; }
    
    public Guid? TopicId { get; set; }
    
    public Guid CapstoneId { get; set; }
    
    public string? TopicCode { get; set; }
    public string GroupCode { get; set; } = string.Empty;
    public GroupStatus Status { get; set; }
    
    public Major Major { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public ICollection<GroupMember> GroupMembers { get; set; } = new List<GroupMember>();
}
