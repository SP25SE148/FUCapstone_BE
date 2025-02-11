using FUC.Common.Events;

namespace FUC.Common.IntegrationEventLog.Services;

public interface IIntegrationEventService
{
    Task PublishEventsAsync(Guid transactionId);
    void SaveEventAsync(IntegrationEvent evt);
}
