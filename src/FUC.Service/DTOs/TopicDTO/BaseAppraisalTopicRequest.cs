using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicDTO;

public class BaseAppraisalTopicRequest
{
    public string AppraisalContent { get; set; }
    public string AppraisalComment { get; set; }
    public TopicAppraisalStatus Status { get; set; }
}
