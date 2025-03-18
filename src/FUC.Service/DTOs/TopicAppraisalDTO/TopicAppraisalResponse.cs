namespace FUC.Service.DTOs.TopicAppraisalDTO;

public sealed class TopicAppraisalResponse
{
    public Guid TopicAppraisalId { get; set; }
    public Guid TopicId { get; set; }
    public string? SupervisorId { get; set; }
    public string? ManagerId { get; set; }
    public string TopicEnglishName { get; set; }
    public string? AppraisalContent { get; set; }
    public string? AppraisalComment { get; set; }
    public int AttemptTime { get; set; }
    public string? Status { get; set; }
    public DateTime? AppraisalDate { get; set; }
    public DateTime CreatedDate { get; set; }
}
