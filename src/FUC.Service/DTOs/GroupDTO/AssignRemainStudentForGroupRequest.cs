namespace FUC.Service.DTOs.GroupDTO;

public class AssignRemainStudentForGroupRequest
{
    public required Guid GroupId { get; set; }
    public required string StudentId { get; set; }
}
