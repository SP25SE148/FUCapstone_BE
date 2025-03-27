using FUC.Data.Abstractions;

namespace FUC.Data.Entities;

public class TimeConfiguration : Entity
{
    public Guid Id { get; set; }
    public DateTime TeamUpDate { get; set; }
    public DateTime TeamUpExpirationDate { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
    public string CampusId { get; set; }

    public Campus Campus { get; set; } = null!;
}
