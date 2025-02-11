using FUC.Common.Events;
using FUC.Common.IntegrationEventLog.Services;
using Identity.API.Data;
using MassTransit;

namespace Identity.API.Services;

public class IdentityIntegrationEventService : IIntegrationEventService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ApplicationDbContext _dbContext;
    private readonly IIntegrationEventLogService _eventLogService;
    private readonly ILogger<IdentityIntegrationEventService> _logger;

    public IdentityIntegrationEventService(IPublishEndpoint publishEndpoint,
        ApplicationDbContext dbContext,
        IIntegrationEventLogService integrationEventLogService,
        ILogger<IdentityIntegrationEventService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _dbContext = dbContext;
        _eventLogService = integrationEventLogService;
        _logger = logger;   
    }

    public void SaveEventAsync(IntegrationEvent evt)
    {
        _logger.LogInformation("Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.Id, evt);

        _eventLogService.SaveEvent(evt, _dbContext.GetCurrentTransaction());
    }

    public async Task PublishEventsAsync(Guid transactionId)
    {
        var pendingLogEvents = await _eventLogService.RetrieveEventLogsPendingToPublishAsync(transactionId);

        foreach (var logEvt in pendingLogEvents)
        {
            _logger.LogInformation("Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})", logEvt.EventId, logEvt.IntegrationEvent);

            try
            {
                _eventLogService.MarkEventAsInProgress(logEvt.EventId);
                await _publishEndpoint.Publish(logEvt.IntegrationEvent);
                _eventLogService.MarkEventAsPublished(logEvt.EventId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing integration event: {IntegrationEventId}", logEvt.EventId);

                _eventLogService.MarkEventAsFailed(logEvt.EventId);
            }
        }
    }
}
