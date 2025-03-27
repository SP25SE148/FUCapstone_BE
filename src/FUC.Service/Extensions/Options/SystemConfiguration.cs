namespace FUC.Service.Extensions.Options;

public class SystemConfiguration
{
    public int MaxTopicsForCoSupervisors { get; set; } = 3;
    public int MaxTopicAppraisalsForTopic { get; set; } = 2;
    public double ExpirationTopicRequestDuration { get; set; } = 1;
    public double ExpirationTeamUpDuration { get; set; } = 1;
    public int MaxAttemptTimesToDefendCapstone { get; set; } = 2;
    public int MaxAttemptTimesToReviewTopic { get; set; } = 3;
}
