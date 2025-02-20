using System.Runtime.InteropServices.JavaScript;
using FUC.Data.Abstractions;
using FUC.Data.Abstractions.Entities;

namespace FUC.Data.Entities;


public sealed class Semester : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // semester code
    public string Name { get; set; } = string.Empty;
    public int  MaxGroupsPerSupervisor { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
