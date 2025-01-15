using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Data;

public sealed class Campus : Entity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public  ICollection<Group> Groups { get; set; } = new List<Group>();
    public  ICollection<Student> Students { get; set; } = new List<Student>();
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
