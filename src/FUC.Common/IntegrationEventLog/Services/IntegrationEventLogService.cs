using System.Text.Json;
using FUC.Common.Events;
using Microsoft.EntityFrameworkCore;

namespace FUC.Common.IntegrationEventLog.Services;

public class IntegrationEventLogService<TContext> : IIntegrationEventLogService, IDisposable
    where TContext : DbContext
{
    private volatile bool _disposedValue;
    private readonly TContext _context;
    private readonly JsonSerializerOptions s_indentedOptions = new() { WriteIndented = true };

    public IntegrationEventLogService(TContext context)
    {
        _context = context;
    }

    public void SendEvent(IntegrationEvent @event)
    {
        var eventLog = new IntegrationEventLog
        {
            Id = @event.Id,
            OccurredOnUtc = @event.CreationDate,
            Type = @event.GetType().FullName!,
            Content = JsonSerializer.Serialize(@event, @event.GetType(), s_indentedOptions)
        };

        _context.Set<IntegrationEventLog>().Add(eventLog);
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
