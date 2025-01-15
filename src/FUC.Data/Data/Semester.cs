using System.Runtime.InteropServices.JavaScript;
using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Data;

public sealed class Semester : Entity, ISoftDelete
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public ICollection<Group> Groups { get; set; } = new List<Group>();
}
