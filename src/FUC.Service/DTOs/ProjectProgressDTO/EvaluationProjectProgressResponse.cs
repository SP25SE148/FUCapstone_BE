using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class EvaluationProjectProgressResponse
{
    public required string StudentCode { get; set; }
    public required string StudentName { get; set; }
    public required string StudentRole { get; set;}
    public double AverageContributionPercentage { get; set; }
    public List<EvaluationWeekResponse> EvaluationWeeks { get; set; }
}

public class EvaluationWeekResponse
{
    public int WeekNumber { get; set; }
    public double ContributionPercentage { get; set; }
    public string? Summary { get; set; }
    public string? MeetingContent { get; set; }
    public string? Comments { get; set; }
    public string Status { get; set; }
}

