namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed class GroupMemberRequestResponse
{
    public IReadOnlyList<GroupMemberResponse>?
        GroupMemberRequestSentByLeader { get; set; } // leader view all group member request was sent by leader 

    public IReadOnlyList<GroupMemberResponse>?
        GroupMemberRequested { get; set; } // member view all group member requested by leader

    public IReadOnlyList<GroupMemberResponse>?
        JoinGroupRequested { get; set; } // leader view all join group requested by member

    public IReadOnlyList<GroupMemberResponse>?
        JoinGroupRequestSentByMember { get; set; } // member view all join group member request was sent by member
}
