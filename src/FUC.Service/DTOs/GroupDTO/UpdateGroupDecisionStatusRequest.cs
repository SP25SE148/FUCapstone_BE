using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupDTO;

public class UpdateGroupDecisionStatusRequest
{
    public Guid GroupId { get; set; }
}

public sealed class UpdateGroupDecisionStatusBySupervisorRequest : UpdateGroupDecisionStatusRequest
{
    public DecisionStatus DecisionStatus { get; set; }
    public string? Comment { get; set; }
}

public sealed class UpdateGroupDecisionStatusByPresidentRequest
{
    public bool IsReDefendCapstoneProject { get; set; }
    public Guid CalendarId { get; set; }
}
