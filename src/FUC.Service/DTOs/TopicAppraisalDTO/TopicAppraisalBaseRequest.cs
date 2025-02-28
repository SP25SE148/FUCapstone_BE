using FUC.Common.Shared;
using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicAppraisalDTO;

public class TopicAppraisalBaseRequest : PaginationParams
{
    public string Status { get; set; } = "all";
    public string? SearchTerm { get; set; }
    public string OrderByAppraisalDate { get; set; } = "_asc";
}
