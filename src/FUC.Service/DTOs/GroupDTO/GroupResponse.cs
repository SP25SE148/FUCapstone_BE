using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.DTOs.GroupDTO;

public sealed record GroupResponse
{
    public Guid Id { get; init; }
    public string SemesterName { get; init; } = string.Empty;
    public string MajorName { get; init; } = string.Empty;
    public string CapstoneName { get; init; } = string.Empty;
    public string CampusName { get; init; } = string.Empty;
    public string? TopicCode { get; init; } = "undefined";
    public string? GroupCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public IEnumerable<GroupMemberResponse> GroupMemberList { get; init; } = new List<GroupMemberResponse>();
    public TopicResponse? TopicResponse { get; set; }
}
