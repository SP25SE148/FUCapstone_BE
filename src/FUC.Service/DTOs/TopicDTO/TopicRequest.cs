using FUC.Common.Shared;

namespace FUC.Service.DTOs.TopicDTO;

public class TopicRequest : PaginationParams
{
    public string MainSupervisorEmail { get; set; } = "all";
    public string? SearchTerm { get; set; }
    public string Status { get; set; } = "all";
    public string DifficultyLevel { get; set; } = "all";
    public string BusinessAreaName { get; set; } = "all";
    public string CapstoneId { get; set; }
    public string SemesterId { get; set; }
    public string CampusId { get; set; }
}
