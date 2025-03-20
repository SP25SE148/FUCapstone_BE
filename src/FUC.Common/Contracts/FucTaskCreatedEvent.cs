using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class FucTaskCreatedEvent : IntegrationEvent
{
    public Guid FucTaskId { get; set; }
    public required string KeyTask { get; set; }
    public required string ReporterName { get; set; }
    public required string ReminderType { get; set; }
    public TimeSpan RemindTimeOnDueDate { get; set; }
    public int RemindInDaysBeforeDueDate { get; set; }
    public required string NotificationFor { get; set; }
}
