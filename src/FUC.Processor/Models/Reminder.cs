namespace FUC.Processor.Models;

public class Reminder
{
    public Guid Id { get; set; }

    public string? Content { get; set; }

    public required string ReminderType { get; set; }

    public required string RemindFor { get; set; }

    public DateTime RemindDate { get; set; }
}
