namespace FUC.Service.DTOs.TopicRequestDTO;

public sealed class TopicRequestResponse
{
    public Guid TopicRequestId { get; set; }
    public string GroupCode { get; set; }
    public Guid GroupId { get; set; }
    public string SupervisorId { get; set; }
    public string SupervisorFullName { get; set; }
    public Guid TopicId { get; set; }
    public string TopicCode { get; set; }
    public string TopicEnglishName { get; set; }
    public string Status { get; set; }
    public string RequestedBy { get; set; }

    public string LeaderFullName { get; set; }
    public DateTime CreatedDate { get; set; }
}
