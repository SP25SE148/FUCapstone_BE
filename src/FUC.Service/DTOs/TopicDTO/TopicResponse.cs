using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicDTO;

public class TopicResponse
{
    public string Id { get; set; }
    public string Code { get; set; }
    public string MainSupervisorId { get; set; }
    public string MainSupervisorName { get; set; }
    public string MainSupervisorEmail { get; set; }
    public string EnglishName { get; set; }
    public string VietnameseName { get; set; }
    public string Abbreviation { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public string Status { get; set; }
    public string DifficultyLevel { get; set; }
    public string BusinessAreaName { get; set; }
    public string CapstoneId { get; set; }
    public string SemesterId { get; set; }
    public string CampusId { get; set; }
    public DateTime CreatedDate { get; set; }
    public List<CoSupervisorDto> CoSupervisors { get; set; }
    public List<TopicAppraisalDto> TopicAppraisals { get; set; } = new();
}

public sealed class CoSupervisorDto
{
    public string SupervisorCode { get; set; }
    public string SupervisorName { get; set; }
    public string SupervisorEmail { get; set; }
}

public sealed class TopicAppraisalDto
{
    public Guid TopicAppraisalId { get; set; }
    public Guid TopicId { get; set; }
    public string SupervisorId { get; set; }
    public string? AppraisalContent { get; set; }
    public string? AppraisalComment { get; set; }
    public TopicAppraisalStatus Status { get; set; }
    public DateTime? AppraisalDate { get; set; }
    public int AttemptTime { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class TopicForStudentResponse : TopicResponse
{
    public int NumberOfTopicRequest { get; set; }
}
