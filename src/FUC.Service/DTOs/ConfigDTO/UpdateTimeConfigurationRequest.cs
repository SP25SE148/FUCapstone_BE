namespace FUC.Service.DTOs.ConfigDTO;

public class UpdateTimeConfigurationRequest
{
    public Guid Id { get; set; }
    public DateTime? TimeUpDate { get; set; }
    public DateTime? TimeUpExpirationDate { get; set; }
    public DateTime? RegistTopicDate { get; set; }
    public DateTime? RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
}

public class CreateTimeConfigurationRequest
{
    public DateTime TimeUpDate { get; set; }
    public DateTime TimeUpExpirationDate { get; set; }
    public DateTime RegistTopicDate { get; set; }
    public DateTime RegistTopicExpiredDate { get; set; }
    public bool IsActived { get; set; }
    public string CapstoneId { get; set; }
}

