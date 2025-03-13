namespace FUC.Service.DTOs.ProjectProgressDTO;

public class SummaryProjectProgressWeekRequest
{
    public Guid ProjectProgressId { get; set; }
    public Guid ProjectProgressWeekId { get; set; }
    public required string Summary { get; set; }
}
