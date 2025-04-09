using FUC.Common.Events;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FUC.Common.Contracts;

public class TimeConfigurationCreatedEvent : IntegrationEvent
{
    public Guid RequestId { get; set; }
    public string CampusId { get; set; }
    public RegistTopicTimeConfigurationCreatedEvent RegistTopicTimeConfigurationCreatedEvent { get; set; }
    public TeamUpTimeConfigurationCreatedEvent TeamUpTimeConfigurationCreatedEvent { get; set; }
    public TimeSpan RemindTime { get; set; }
    public int RemindInDaysBeforeDueDate { get; set; }
}

public class TeamUpTimeConfigurationCreatedEvent : TimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime TeamUpDate { get; set; }
    public DateTime TeamUpExpirationDate { get; set; }
}

public class RegistTopicTimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
}
