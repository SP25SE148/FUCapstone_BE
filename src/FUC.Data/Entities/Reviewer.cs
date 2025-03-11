using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class Reviewer : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string SupervisorId { get; set; }
    public Guid ReviewCalenderId { get; set; }
    public string? Suggestion { get; set; }
    public string? Comment { get; set; }
    public Supervisor Supervisor { get; set; } = null!;
    public ReviewCalendar ReviewCalender { get; set; } = null!;
}
