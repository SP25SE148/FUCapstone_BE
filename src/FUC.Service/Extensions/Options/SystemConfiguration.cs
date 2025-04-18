namespace FUC.Service.Extensions.Options;

public class SystemConfiguration
{
    public int MaxTopicsForCoSupervisors { get; set; } = 3;
    public int MaxTopicAppraisalsForTopic { get; set; } = 2;
    public double ExpirationTopicRequestDuration { get; set; } = 1; // 1 day
    public double ExpirationTeamUpDuration { get; set; } = 1; // 1 day
    public double ExpirationReviewCalendarDuration { get; set; } = 1; // 1 day
    public int MaxAttemptTimesToDefendCapstone { get; set; } = 2;
    public int MaxAttemptTimesToReviewTopic { get; set; } = 3;
    public Dictionary<string, Dictionary<string, double>> MininumTopicsPerCapstoneInEachCampus { get; set; } // base on students
}
