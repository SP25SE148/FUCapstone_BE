namespace FUC.Processor.Models;

public class Notification
{
    public Guid Id { get; set; }
    public bool IsRead { get; set; }
    public string UserEmail { get; set; }
    public string Content { get; set; }
    public string Type { get; set; }
    public DateTime CreatedDate { get; set; }
}
