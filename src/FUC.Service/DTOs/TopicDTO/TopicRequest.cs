using FUC.Common.Shared;

namespace FUC.Service.DTOs.TopicDTO;

public class TopicRequest : PaginationParams
{
    public string MainSupervisorEmail { get; set; } = "all";
    public string? SearchTerm { get; set; }
    public string Status { get; set; } = "all";
    public string DifficultyLevel { get; set; } = "all";
    public string BusinessAreaId { get; set; } = "all";
    public string CapstoneId { get; set; } = "all";
    public string SemesterId { get; set; } = "all";
    public string CampusId { get; set; } = "all";
}
