using FUC.Common.Events;

namespace FUC.Common.Contracts;

public sealed class ReviewCalendarExpirationEvent : IntegrationEvent
{
    public Guid ReviewCalendarId { get; set; }
}
