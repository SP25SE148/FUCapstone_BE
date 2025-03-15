using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record UpdateJoinGroupRequest(Guid Id, JoinGroupRequestStatus Status);
