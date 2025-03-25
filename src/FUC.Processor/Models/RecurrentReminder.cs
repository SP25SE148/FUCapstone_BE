namespace FUC.Processor.Models;

public class RecurrentReminder
{
    public Guid Id { get; set; }

    public string? Content { get; set; } 

    public required string ReminderType { get; set; }

    public string RemindFor { get; set; } = null!;

    public DayOfWeek RecurringDay { get; set; }

    public DateTime? EndDate { get; set; }

    public TimeSpan RemindTime { get; set; }
}
