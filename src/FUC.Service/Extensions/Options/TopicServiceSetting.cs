namespace FUC.Service.Extensions.Options;

public class TopicServiceSetting
{
    public int MaxTopicsForCoSupervisors { get; set; } = 3;
    public int MaxTopicAppraisalsForTopic { get; set; } = 2;
}
