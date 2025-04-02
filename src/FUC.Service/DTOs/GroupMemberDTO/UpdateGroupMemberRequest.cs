using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record UpdateGroupMemberRequest(
    Guid Id,
    Guid GroupId,
    GroupMemberStatus Status);
