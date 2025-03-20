using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class DefendCapstoneProjectCouncilMember : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid DefendCapstoneProjectInformationCalendarId { get; set; }
    public string SupervisorId { get; set; }
    public bool IsPresident { get; set; }
    public bool IsSecretary { get; set; }

    public DefendCapstoneProjectInformationCalendar DefendCapstoneProjectInformationCalendar { get; set; } = null!;
    public Supervisor Supervisor { get; set; } = null!;
}
