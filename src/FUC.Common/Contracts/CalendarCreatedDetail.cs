namespace FUC.Common.Contracts;

public class CalendarCreatedDetail
{
    public Guid CalendarId { get; set; }
    public List<string> Users { get; set; }
    public string Type { get; set; }
    public DateTime StartDate { get; set; }
}
