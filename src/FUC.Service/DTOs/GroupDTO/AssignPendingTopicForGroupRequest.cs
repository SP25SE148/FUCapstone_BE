namespace FUC.Service.DTOs.GroupDTO;

public class AssignPendingTopicForGroupRequest
{
    public Guid TopicId { get; set; }
    public Guid GroupId { get; set; }
}
