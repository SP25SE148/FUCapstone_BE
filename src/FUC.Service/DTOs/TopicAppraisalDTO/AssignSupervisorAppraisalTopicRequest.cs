namespace FUC.Service.DTOs.TopicAppraisalDTO;

public class AssignSupervisorAppraisalTopicRequest
{
    public Guid TopicId { get; set; }
    public required string SupervisorId { get; set; }
}

public class RemoveAssignSupervisorAppraisalTopicRequest
{
    public Guid TopicId { get; set; }
    public Guid TopicAppraisalId { get; set; }
}
