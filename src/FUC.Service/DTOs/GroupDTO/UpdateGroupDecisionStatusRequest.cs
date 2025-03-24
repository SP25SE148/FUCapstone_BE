using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupDTO;

public class UpdateGroupDecisionStatusRequest
{
    public Guid GroupId { get; set; }
}

public sealed class UpdateGroupDecisionStatusBySupervisorRequest : UpdateGroupDecisionStatusRequest
{
    public DecisionStatus DecisionStatus { get; set; }
}

public sealed class UpdateGroupDecisionStatusByPresidentRequest : UpdateGroupDecisionStatusRequest
{
    public bool IsReDefendCapstoneProject { get; set; }
}
