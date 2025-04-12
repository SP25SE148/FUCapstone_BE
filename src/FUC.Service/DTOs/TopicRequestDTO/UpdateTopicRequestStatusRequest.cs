using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicRequestDTO;

public sealed class UpdateTopicRequestStatusRequest
{
    public Guid TopicRequestId { get; set; }
    public TopicRequestStatus Status { get; set; }
    public string? Reason { get; set; } = "";
}
