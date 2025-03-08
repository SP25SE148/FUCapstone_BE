using FUC.Common.Shared;

namespace FUC.Service.DTOs.TopicDTO;

public class TopicForGroupParams : PaginationParams
{
    public string MainSupervisorEmail { get; set; } = "all";
    public string? SearchTerm { get; set; }
    public string DifficultyLevel { get; set; } = "all";
    public string BusinessAreaId { get; set; } = "all";
}
