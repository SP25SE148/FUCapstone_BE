using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public class ReviewCalendar : AuditableSoftDeleteEntity
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public Guid GroupId { get; set; }
    public string MajorId { get; set; }
    public string CampusId { get; set; }
    public string SemesterId { get; set; }
    public int Attempt { get; set; }
    public int Slot { get; set; }
    public string Room { get; set; }
    public ReviewCalendarStatus Status { get; set; }

    public Topic Topic { get; set; } = null!;
    public Group Group { get; set; } = null!;
    public Major Major { get; set; } = null!;
    public Campus Campus { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
    public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
}
