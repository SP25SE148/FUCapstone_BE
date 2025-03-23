using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class FucTaskAssigneeUpdatedEvent : IntegrationEvent
{
    public Guid FucTaskId { get; set; }
    public Guid ProjectProgressId { get; set; }
    public required string ReminderType { get; set; }
    public required string NotificationFor { get; set; }
}

public class FucTaskDueDateUpdatedEvent : IntegrationEvent
{
    public Guid FucTaskId { get; set; }
    public Guid ProjectProgressId { get; set; }
    public required string ReminderType { get; set; }
    public TimeSpan DueDateChangedTime { get; set; }
}

public class FucTaskStatusDoneUpdatedEvent : IntegrationEvent
{
    public Guid FucTaskId { get; set; }
    public Guid ProjectProgressId { get; set; }
    public required string ReminderType { get; set; }
}
