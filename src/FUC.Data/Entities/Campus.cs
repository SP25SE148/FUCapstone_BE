using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class Campus : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // campus code
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Student> Students { get; set; } = new List<Student>();
    public ICollection<Supervisor> Supervisors { get; set; } = new List<Supervisor>();
    public ICollection<ReviewCalendar> ReviewCalendars { get; set; } = new List<ReviewCalendar>();

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();

    public ICollection<DefendCapstoneProjectInformationCalendar>
        DefendCapstoneProjectInformationCalendars { get; set; } = new List<DefendCapstoneProjectInformationCalendar>();
    public ICollection<TimeConfiguration> TimeConfigurations { get; set; } = new List<TimeConfiguration>();

}
