using System.Collections;
using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class Topic : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public string MainSupervisorId { get; set; }
    public string CapstoneId { get; set; }
    public string SemesterId { get; set; }
    public string CampusId { get; set; }
    public Guid BusinessAreaId { get; set; }

    public string? Code { get; set; }
    public string EnglishName { get; set; }
    public string VietnameseName { get; set; }
    public string Abbreviation { get; set; }
    public string Description { get; set; }
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public TopicStatus Status { get; set; }
    public DifficultyLevel DifficultyLevel { get; set; }

    public Supervisor MainSupervisor { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public BusinessArea BusinessArea { get; set; } = null!;

    public ICollection<CoSupervisor> CoSupervisors { get; set; } = new List<CoSupervisor>();
    public ICollection<TopicAppraisal> TopicAppraisals { get; set; } = new List<TopicAppraisal>();
}
