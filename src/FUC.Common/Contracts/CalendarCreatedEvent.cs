using FUC.Common.Events;

namespace FUC.Common.Contracts;

public class CalendarCreatedEvent : IntegrationEvent
{
    public List<CalendarCreatedDetail> Details { get; set; }
}

public class CalendarOnTimeEvent : IntegrationEvent
{
    public Guid CalendarId { get; set; }
    public string Type { get; set; }
    public string Status { get; set; }
}
