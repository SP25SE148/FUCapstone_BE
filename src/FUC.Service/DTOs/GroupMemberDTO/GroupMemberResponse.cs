using FUC.Data.Enums;

namespace FUC.Service.DTOs.GroupMemberDTO;

public sealed class GroupMemberResponse
{
    public Guid Id { get; set; }
    public Guid GroupId { get; set; }
    public string StudentId { get; set; }
    public string StudentEmail { get; set; }
    public string StudentFullName { get; set; }
    public float GPA { get; set; }
    public bool IsLeader { get; set; }
    public bool IsRequestFromLeader { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string Status { get; set; }
}
