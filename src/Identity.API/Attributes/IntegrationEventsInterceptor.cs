using FUC.Common.IntegrationEventLog.Services;
using MassTransit;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Identity.API.Attributes;

public class IntegrationEventsInterceptor : SaveChangesInterceptor
{
    private Guid? _transactionId;
    private readonly IIntegrationEventService _eventService;

    public IntegrationEventsInterceptor(IIntegrationEventService eventService)
    {
        _eventService = eventService;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var context = eventData.Context;
        if (context?.Database.CurrentTransaction != null)
        {
            _transactionId = context.Database.CurrentTransaction.TransactionId;
        }

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null || !_transactionId.HasValue)
        {
            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        await _eventService.PublishEventsAsync((Guid)_transactionId);

        // Reset transaction ID
        _transactionId = null;

        return await base.SavedChangesAsync(eventData, result, cancellationToken);
    }
}
