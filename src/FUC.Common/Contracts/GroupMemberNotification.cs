namespace FUC.Common.Contracts;

public sealed class GroupMemberNotification
{
    public string MemberId { get; set; }
    public Guid GroupId { get; set; }
    public Guid GroupMemberId { get; set; }
}
