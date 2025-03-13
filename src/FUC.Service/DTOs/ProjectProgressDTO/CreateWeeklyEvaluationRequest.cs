using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class CreateWeeklyEvaluationRequest
{
    public Guid ProjectProgressId { get; set; }
    public Guid ProjectProgressWeekId { get; set; }
    public List<WeeklyEvaluationRequestForStudent> EvaluationRequestForStudents { get; set; } = new();
}

public class WeeklyEvaluationRequestForStudent
{
    public string StudentId { get; set; }
    public double ContributionPercentage { get; set; }
    public string Comments { get; set; }
    public EvaluationStatus Status { get; set; }
}
