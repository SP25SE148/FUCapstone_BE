using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record GroupMemberResponse(
    Guid Id,
    Guid GroupId,
    string StudentId,
    string GroupName,
    GroupMemberStatus Status
    );
