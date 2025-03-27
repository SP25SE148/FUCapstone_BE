using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class TimeConfiguration : Entity
{
    public Guid Id { get; set; }
    public DateTime TimeUpDate { get; set; }
    public DateTime TimeUpExpirationDate { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
    public string SemesterId { get; set; }
    public string CapstoneId { get; set; }

    public Semester Semester { get; set; } = null!;
    public Capstone Capstone { get; set; } = null!;
}
