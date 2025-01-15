using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Data;

public sealed class Major : Entity, ISoftDelete
{
    public Guid Id { get; set; }
    public Guid MajorGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }

    public MajorGroup MajorGroup { get; set; } = null!;
    public ICollection<Capstone> Capstones { get; set; } = new List<Capstone>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
