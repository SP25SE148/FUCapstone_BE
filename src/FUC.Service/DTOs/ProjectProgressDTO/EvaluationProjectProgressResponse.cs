namespace FUC.Service.DTOs.ProjectProgressDTO;

public class EvaluationProjectProgressResponse
{
    public string StudentCode { get; set; }
    public string StudentName { get; set; }
    public string StudentRole { get; set;}
    public double AverageContributionPercentage { get; set; }
    public List<EvaluationWeekResponse> EvaluationWeeks { get; set; }
}

public class EvaluationWeekResponse
{
    public int WeekNumber { get; set; }
    public double ContributionPercentage { get; set; }
    public string TaskDescription { get; set; }
    public string MeetingContent { get; set; }
    public string Comments { get; set; }
}

