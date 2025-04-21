using FUC.Common.Events;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace FUC.Common.Contracts;

public class TimeConfigurationCreatedEvent : IntegrationEvent
{
    public Guid RequestId { get; set; }
    public string CampusId { get; set; }

    public RegistTopicForSupervisorTimeConfigurationCreatedEvent RegistTopicForSupervisorTimeConfigurationCreatedEvent
    {
        get;
        set;
    }

    public TeamUpTimeConfigurationCreatedEvent TeamUpTimeConfigurationCreatedEvent { get; set; }

    public RegistTopicForGroupTimeConfigurationCreatedEvent RegistTopicForGroupTimeConfigurationCreatedEvent
    {
        get;
        set;
    }

    public ReviewAttemptTimeConfigurationCreatedEvent ReviewAttemptTimeConfigurationCreatedEvent { get; set; }

    public DefendCapstoneProjectTimeConfigurationCreatedEvent DefendCapstoneProjectTimeConfigurationCreatedEvent
    {
        get;
        set;
    }

    public TimeSpan RemindTime { get; set; }
    public int RemindInDaysBeforeDueDate { get; set; }
}

public class TeamUpTimeConfigurationCreatedEvent : TimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime TeamUpDate { get; set; }
    public DateTime TeamUpExpirationDate { get; set; }
}

public class RegistTopicForSupervisorTimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
}

public class RegistTopicForGroupTimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime RegistTopicForGroupDate { get; set; }
    public DateTime RegistTopicForGroupExpiredDate { get; set; }
}

public class ReviewAttemptTimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime ReviewAttemptDate { get; set; }
    public DateTime ReviewAttemptExpiredDate { get; set; }
}

public class DefendCapstoneProjectTimeConfigurationCreatedEvent
{
    public string NotificationFor { get; set; }
    public DateTime DefendCapstoneProjectDate { get; set; }
    public DateTime DefendCapstoneProjectExpiredDate { get; set; }
}
