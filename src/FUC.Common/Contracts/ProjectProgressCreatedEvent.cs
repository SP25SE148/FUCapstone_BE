using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class ProjectProgressCreatedEvent : IntegrationEvent
{
    public Guid GroupId { get; set; }
    public string SupervisorCode { get; set; }
    public string SupervisorName { get; set; }
    public Guid ProjectProgressId { get; set; }
    public required List<string> StudentCodes { get; set; } = new();
    public required string Type { get; set; }
    public DayOfWeek RemindDate { get; set; }
    public TimeSpan RemindTime { get; set; }
    public DateTime? EndDate { get; set; }
}

public class ProjectProgressUpdatedEvent : IntegrationEvent
{
    public Guid GroupId { get; set; }
    public Guid ProjectProgressId { get; set; }
    public required string Type { get; set; }
    public DayOfWeek RemindDate { get; set; }
    public required List<string> StudentCodes { get; set; } = new();
}
