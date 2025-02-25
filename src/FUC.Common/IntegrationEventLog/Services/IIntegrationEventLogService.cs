using FUC.Common.Events;

namespace FUC.Common.IntegrationEventLog.Services;

public interface IIntegrationEventLogService
{
    void SendEvent(IntegrationEvent @event);
}
