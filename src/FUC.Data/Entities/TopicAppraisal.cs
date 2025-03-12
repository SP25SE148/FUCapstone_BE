using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class TopicAppraisal : AuditableEntity
{
    public Guid Id { get; set; }
    public required string SupervisorId { get; set; }
    public Guid TopicId { get; set; }
    public string? AppraisalContent { get; set; }
    public string? AppraisalComment { get; set; }
    public TopicAppraisalStatus Status { get; set; }
    public DateTime? AppraisalDate { get; set; }

    public Supervisor Supervisor { get; set; } = null!;
    public Topic Topic { get; set; } = null!;
}
