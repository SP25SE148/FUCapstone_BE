using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class TopicAppraisal : Entity
{
    public Guid Id { get; set; }
    public string? SupervisorId { get; set; }
    public string? ManagerEmail { get; set; }    
    public Guid TopicId { get; set; }
    public string AppraisalContent { get; set; }
    public string AppraisalComment { get; set; }
    public TopicAppraisalStatus Status { get; set; }
    public DateTime AppraisalDate { get; set; }

    public Supervisor? Supervisor { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}
