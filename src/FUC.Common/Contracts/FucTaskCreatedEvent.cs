using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class FucTaskCreatedEvent : IntegrationEvent
{
    public Guid FucTaskId { get; set; }
    public required string ReminderType { get; set; }
    public TimeSpan RemindTimeOnDueDate { get; set; }
    public TimeSpan RemindInDaysBeforeDueDate { get; set; }
    public string NotificationFor { get; set; }
}
