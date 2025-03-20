using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public sealed class DefendCapstoneProjectInformationCalendar : AuditableEntity
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public string CampusId { get; set; }
    public string SemesterId { get; set; }
    public int DefendAttempt { get; set; }
    public string Location { get; set; } // Room
    public string FileName { get; set; }
    public string FileUrl { get; set; }
    public DateTime Date { get; set; }

    public Campus Campus { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
    public Topic Topic { get; set; } = null!;

    public ICollection<DefendCapstoneProjectCouncilMember> DefendCapstoneProjectMemberCouncils { get; set; } =
        new List<DefendCapstoneProjectCouncilMember>();
}
