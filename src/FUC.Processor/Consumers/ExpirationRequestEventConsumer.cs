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
        _logger.LogInformation("--> Consume expiration event for {RequestTyp} with Id {Id} - Event {EventId}",
            message.RequestType,
            message.RequestId, 
            message.Id);

        var expirationTask = new Reminder
        {
            RemindFor = message.RequestId.ToString(),
            ReminderType = message.RequestType,
            RemindDate = DateTime.SpecifyKind(DateTime.Now.Add(message.ExpirationDuration), DateTimeKind.Utc),
        };

        _processorDb.Reminders.Add(expirationTask);

        await _processorDb.SaveChangesAsync();  
    }
}
