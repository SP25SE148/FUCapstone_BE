namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record CreateGroupMemberRequest(
    IReadOnlyList<string> MemberEmailList
);
