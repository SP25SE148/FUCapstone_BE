using FUC.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace FUC.Common.IntegrationEventLog.Services;

public interface IIntegrationEventLogService
{
    Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
    void SaveEvent(IntegrationEvent @event, IDbContextTransaction transaction);
    void MarkEventAsPublished(Guid eventId);
    void MarkEventAsInProgress(Guid eventId);
    void MarkEventAsFailed(Guid eventId);
}
