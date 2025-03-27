namespace FUC.Service.DTOs.ConfigDTO;

public class TimeConfigurationDto
{
    public Guid Id { get; set; }
    public DateTime TeamUpDate { get; set; }
    public DateTime TeamUpExpirationDate { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
    public string CampusId { get; set; }
}
