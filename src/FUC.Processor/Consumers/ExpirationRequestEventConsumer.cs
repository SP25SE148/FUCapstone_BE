using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class ExpirationRequestEventConsumer : BaseEventConsumer<ExpirationRequestEvent>
{
    private readonly ILogger<ExpirationRequestEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDb;

    public ExpirationRequestEventConsumer(ILogger<ExpirationRequestEventConsumer> logger,
        ProcessorDbContext processorDb,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _processorDb = processorDb;
        _logger = logger;
    }

    protected override async Task ProcessMessage(ExpirationRequestEvent message)
    {
        try
        {
            _logger.LogInformation("--> Consume expiration event for {RequestType} with Id {Id} - Event {EventId}",
            message.RequestType,
            message.RequestId,
            message.Id);

            var expirationTask = new Reminder
            {
                RemindFor = message.RequestId.ToString(),
                ReminderType = message.RequestType,
                RemindDate = DateTime.Now.Add(message.ExpirationDuration),
            };

            _processorDb.Reminders.Add(expirationTask);

            await _processorDb.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to consume expiration event with EventId {Id} with error {Message}.", message.Id, ex.Message);
            throw;
        }
    }
}
