namespace FUC.Service.DTOs.ProjectProgressDTO;

public class UpdateProjectProgressWeekRequest
{
    public Guid ProjectProgressWeekId { get; set; }
    public string TaskDescription { get; set; }
    public string? MeetingLocation { get; set; }
    public string? MeetingContent { get; set; }
}
