using FUC.Data.Enums;
using Microsoft.AspNetCore.Http;

namespace FUC.Service.DTOs.TopicDTO;
public class CreateTopicRequest
{
    public string CapstoneId { get; set; }
    public Guid BusinessAreaId { get; set; }
    public string EnglishName { get; set; }
    public string VietnameseName { get; set; }
    public string Abbreviation { get; set; }
    public string Description { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }
    public IFormFile File { get; set; }
}
