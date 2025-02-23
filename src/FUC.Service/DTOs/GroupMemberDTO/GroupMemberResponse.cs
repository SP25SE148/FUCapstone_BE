using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed class GroupMemberResponse
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string StudentId { get; set; }
    public string LeaderEmail { get; set; }
    public string StudentFullName { get; set; }
    public string Status { get; set; }
}
