using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.TopicDTO;

namespace FUC.Service.DTOs.DefendCapstone;

public class DefendCapstoneCalendarResponse
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicCode { get; set; }
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public string CampusId { get; set; }
    public string SemesterId { get; set; }
    public string CapstoneId { get; set; }
    public int DefendAttempt { get; set; }
    public string Location { get; set; } // Room
    public int Slot { get; set; }
    public DateTime DefenseDate { get; set; }
    public string Status { get; set; }
    public List<DefendCapstoneCouncilMemberDto> CouncilMembers { get; set; }
}

public class DefendCapstoneCouncilMemberDto
{
    public Guid Id { get; set; }
    public Guid DefendCapstoneProjectInformationCalendarId { get; set; }
    public string SupervisorId { get; set; }
    public string SupervisorName { get; set; }
    public bool IsPresident { get; set; }
    public bool IsSecretary { get; set; }
}

public class DefendCapstoneCalendarDetailResponse : DefendCapstoneCalendarResponse
{
    public string SupervisorId { get; set; }
    public string SupervisorName { get; set; }
    public string TopicEngName { get; set; }
    public string TopicVietName { get; set; }
    public string Abbreviation { get; set; }
    public string Description { get; set; }
}

public class DefendCapstoneResultResponse : DefendCapstoneCalendarResponse
{
    public string GroupStatus { get; set; }
    public bool IsReDefendCapstone { get; set; }
}
