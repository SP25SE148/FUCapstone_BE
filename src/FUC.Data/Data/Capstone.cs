using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Data;

public sealed class Capstone : Entity, ISoftDelete
{
    public Guid Id { get; set; }
    public Guid MajorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int MinMember { get; set; }
    public int MaxMember { get; set; }
    public int ReviewCount { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    public Major Major { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    
}
