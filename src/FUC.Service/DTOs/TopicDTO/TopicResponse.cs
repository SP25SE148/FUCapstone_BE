using FUC.Data.Enums;

namespace FUC.Service.DTOs.TopicDTO;

public class TopicResponse
{
    public string Code { get; set; }
    public string MainSupervisorName { get; set; }
    public string MainSupervisorEmail { get; set; }
    public string EnglishName { get; set; }
    public string VietnameseName { get; set; }
    public string Abbreviation { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public TopicStatus Status { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public string BusinessAreaName { get; set; }
    public string CapstoneId { get; set; }
    public string SemesterId { get; set; }
    public string CampusId { get; set; }
}
