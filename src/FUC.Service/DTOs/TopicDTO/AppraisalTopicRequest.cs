using FUC.Data.Enums;
namespace FUC.Service.DTOs.TopicDTO;

public class AppraisalTopicRequest : BaseAppraisalTopicRequest
{
    public Guid TopicAppraisalId { get; set; }
}
