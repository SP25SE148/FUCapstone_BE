using FUC.Data.Abstractions;
using FUC.Data.Enums;

namespace FUC.Data.Entities;

public sealed class DefendCapstoneProjectInformationCalendar : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public required string TopicCode { get; set; }
    public string CampusId { get; set; }
    public string SemesterId { get; set; }
    public string CapstoneId { get; set; }
    public int DefendAttempt { get; set; }
    public string Location { get; set; } // Room
    public string Time { get; set; }
    public bool IsUploadedThesisMinute { get; set; }
    public DefendCapstoneProjectCalendarStatus Status { get; set; }
    public DateTime DefenseDate { get; set; }
    public Campus Campus { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
    public Topic Topic { get; set; } = null!;

    public ICollection<DefendCapstoneProjectCouncilMember>
        DefendCapstoneProjectMemberCouncils { get; set; } = new List<DefendCapstoneProjectCouncilMember>();
}
