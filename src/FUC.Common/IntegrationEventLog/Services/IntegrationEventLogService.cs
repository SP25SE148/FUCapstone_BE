using FUC.Common.Events;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FUC.Common.IntegrationEventLog.Services;

public class IntegrationEventLogService<TContext> : IIntegrationEventLogService, IDisposable
    where TContext : DbContext
{
    private volatile bool _disposedValue;
    private readonly TContext _context;
    private readonly Type[] _eventTypes;

    public IntegrationEventLogService(TContext context)
    {
        _context = context;
        _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
            .GetTypes()
            //.Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
            .Where(t => typeof(IntegrationEvent).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
            .ToArray();
    }

    public async Task<IEnumerable<IntegrationEventLog>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var result = await _context.Set<IntegrationEventLog>()
            .Where(e => e.TransactionId == transactionId && e.State == EventState.NotPublished)
            .ToListAsync();

        if (result.Count != 0)
        {
            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_eventTypes.FirstOrDefault(t => t.Name == e.EventTypeName)));
        }

        return [];
    }

    public void SaveEvent(IntegrationEvent @event, IDbContextTransaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        var eventLog = new IntegrationEventLog(@event, transaction.TransactionId);

        _context.Database.UseTransaction(transaction.GetDbTransaction());
        _context.Set<IntegrationEventLog>().Add(eventLog);
    }

    public void MarkEventAsPublished(Guid eventId)
    {
        UpdateEventStatus(eventId, EventState.Published);
    }

    public void MarkEventAsInProgress(Guid eventId)
    {
        UpdateEventStatus(eventId, EventState.InProgress);
    }

    public void MarkEventAsFailed(Guid eventId)
    {
        UpdateEventStatus(eventId, EventState.PublishedFailed);
    }

    private void UpdateEventStatus(Guid eventId, EventState status)
    {
        var eventLogEntry = _context.Set<IntegrationEventLog>().Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventState.InProgress)
            eventLogEntry.TimesSent++;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                _context.Dispose();
            }


            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
