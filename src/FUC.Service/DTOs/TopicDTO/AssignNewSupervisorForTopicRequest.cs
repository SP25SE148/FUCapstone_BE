namespace FUC.Service.DTOs.TopicDTO;

public class AssignNewSupervisorForTopicRequest
{
    public string SupervisorId { get; set; }

    public Guid TopicId { get; set; }
}

public class RemoveCoSupervisorForTopicRequest
{
    public string SupervisorId { get; set; }

    public Guid TopicId { get; set; }
}
