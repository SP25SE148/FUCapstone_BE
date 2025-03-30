using FUC.Data.Enums;

namespace FUC.Service.DTOs.ProjectProgressDTO;

public class ProjectProgressDto
{
    public Guid Id { get; set; }
    public string MeetingDate { get; set; }
    public string Slot { get; set; }
    public ICollection<ProjectProgressWeekDto> ProjectProgressWeeks { get; set; }
}

public class ProjectProgressWeekDto
{
    public Guid Id { get; set; }
    public int WeekNumber { get; set; }
    public string TaskDescription { get; set; }
    public string? Summary { get; set; }
    public ProjectProgressWeekStatus Status { get; set; }
    public string? MeetingLocation { get; set; }
    public string? MeetingContent { get; set; }
}

public class UpdateProjectProgressRequest
{
    public Guid Id { get; set; }
    public string? MeetingDate { get; set; }
    public string? Slot { get; set; }
}
