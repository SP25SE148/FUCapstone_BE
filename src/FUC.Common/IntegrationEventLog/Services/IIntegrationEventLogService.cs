using FUC.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;

namespace FUC.Common.IntegrationEventLog.Services;

public interface IIntegrationEventLogService
{
    void SendEvent(IntegrationEvent @event);
}
