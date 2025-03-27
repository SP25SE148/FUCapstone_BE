using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class Capstone : AuditableSoftDeleteEntity
{
    public string Id { get; set; } // capstone code 
    public string MajorId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinMember { get; set; }
    public int MaxMember { get; set; }
    public int ReviewCount { get; set; }
    public int DurationWeeks { get; set; }

    public Major Major { get; set; } = null!;
    public ICollection<Group> Groups { get; set; } = new List<Group>();
    public ICollection<Student> Students { get; set; } = new List<Student>();

    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
    public ICollection<ReviewCriteria> ReviewCriterias { get; set; } = new List<ReviewCriteria>();
    public ICollection<TimeConfiguration> TimeConfigurations { get; set; } = new List<TimeConfiguration>();

}
