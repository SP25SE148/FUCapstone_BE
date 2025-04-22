namespace FUC.Service.DTOs.ConfigDTO;

public class TimeConfigurationDto
{
    public Guid Id { get; set; }

    public string SemesterId { get; set; }

    public string SemesterName { get; set; }

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
}
