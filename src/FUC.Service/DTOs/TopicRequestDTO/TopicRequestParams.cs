using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicRequestDTO;

public sealed class TopicRequestParams
{
    public string SearchTerm { get; set; }
    public string Status { get; set; } = "all";
    public string OrderBy { get; set; } = "_asc";
}
