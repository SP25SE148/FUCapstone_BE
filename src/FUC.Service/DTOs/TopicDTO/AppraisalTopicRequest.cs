using FUC.Data.Enums;
namespace FUC.Service.DTOs.TopicDTO;

public class AppraisalTopicRequest 
{
    public Guid TopicAppraisalId { get; set; }
    public required string AppraisalContent { get; set; }
    public required string AppraisalComment { get; set; }
    public TopicAppraisalStatus Status { get; set; }
}
