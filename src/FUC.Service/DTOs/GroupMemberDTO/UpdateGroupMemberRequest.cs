using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record UpdateGroupMemberRequest(
    Guid Id,
    string? MemberId,
    string? NewLeaderId,
    Guid GroupId,
    GroupMemberStatus Status);
