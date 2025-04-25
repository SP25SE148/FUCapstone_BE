using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class TimeConfiguration : Entity
{
    public Guid Id { get; set; }

    public string SemesterId { get; set; }

    // flow 1
    public DateTime TeamUpDate { get; set; }

    public DateTime TeamUpExpirationDate { get; set; }

    // flow 2
    public DateTime RegistTopicForSupervisorDate { get; set; }
    public DateTime RegistTopicForSupervisorExpiredDate { get; set; }
    // flow 3

    public DateTime RegistTopicForGroupDate { get; set; }
    public DateTime RegistTopicForGroupExpiredDate { get; set; }

    // flow 5
    public DateTime ReviewAttemptDate { get; set; }
    public DateTime ReviewAttemptExpiredDate { get; set; }

    // flow 6
    public DateTime DefendCapstoneProjectDate { get; set; }
    public DateTime DefendCapstoneProjectExpiredDate { get; set; }

    public bool IsActived { get; set; }
    public string CampusId { get; set; }
    public Campus Campus { get; set; } = null!;
    public Semester Semester { get; set; } = null!;
}
