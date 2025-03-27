namespace FUC.Service.DTOs.ConfigDTO;

public class TimeConfigurationDto
{
    public Guid Id { get; set; }
    public DateTime TimeUpDate { get; set; }
    public DateTime TimeUpExpirationDate { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
    public string SemesterId { get; set; }
    public string CapstoneId { get; set; }
}
