using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Entities;


public sealed class Major : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // Major code
    public string MajorGroupId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public MajorGroup MajorGroup { get; set; } = null!;
    public ICollection<Capstone> Capstones { get; set; } = new List<Capstone>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();

}
