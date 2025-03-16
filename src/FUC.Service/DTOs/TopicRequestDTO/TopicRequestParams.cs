using FUC.Common.Shared;
using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicRequestDTO;

public sealed class TopicRequestParams : PaginationParams
{
    public string? SearchTerm { get; set; }
    public TopicRequestStatus? Status { get; set; }
}
