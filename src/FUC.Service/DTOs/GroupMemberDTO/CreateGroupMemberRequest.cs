namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed record CreateGroupMemberRequest(
    string LeaderId,
    IReadOnlyList<string> MemberIdList
);
