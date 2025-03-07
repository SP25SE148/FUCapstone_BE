using FUC.Data.Entities;
using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class CreateWeeklyEvaluationRequest
{
    public string StudentId { get; set; }
    public Guid ProjectProgressWeekId { get; set; }
    public double ContributionPercentage { get; set; }
    public string Comments { get; set; }
    public EvaluationStatus Status { get; set; }
}
