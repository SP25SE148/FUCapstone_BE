using System.Collections;
using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Entities;

public sealed class Campus : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // campus code
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public  ICollection<Group> Groups { get; set; } = new List<Group>();
    public  ICollection<Student> Students { get; set; } = new List<Student>();
    public  ICollection<Supervisor> Supervisors { get; set; } = new List<Supervisor>();

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
