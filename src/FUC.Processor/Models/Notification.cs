namespace FUC.Processor.Models;

public class Notification
{
    public Guid Id { get; set; }
    public bool IsRead { get; set; }
    public required string UserCode { get; set; }
    public string? Content { get; set; }
    public string? Type { get; set; }
    public string? ReferenceTarget { get; set; }
    public DateTime CreatedDate { get; set; }
}
