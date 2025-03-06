namespace FUC.Service.DTOs.TopicRequestDTO;

public sealed class TopicRequestResponse
{
    public string GroupCode { get; set; }
    public Guid GroupId { get; set; }
    public Guid TopicId { get; set; }
    public string Status { get; set; }
    public string RequestedBy { get; set; }
    public DateTime CreatedDate { get; set; }
}
