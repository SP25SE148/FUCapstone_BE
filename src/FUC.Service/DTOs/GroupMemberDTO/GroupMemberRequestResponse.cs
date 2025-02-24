namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed class GroupMemberRequestResponse
{
    public IReadOnlyList<GroupMemberResponse>? GroupMemberRequestSentByLeader  { get; set; }
    public IReadOnlyList<GroupMemberResponse>? GroupMemberRequested  { get; set; }
    
}
