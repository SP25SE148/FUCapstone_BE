using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class CalendarCreatedEventConsumer : BaseEventConsumer<CalendarCreatedEvent>
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<CalendarCreatedEventConsumer> _logger;

    public CalendarCreatedEventConsumer(
        ILogger<CalendarCreatedEventConsumer> logger,
        ProcessorDbContext processorDb,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _processorDbContext = processorDb;
    }

    protected override async Task ProcessMessage(CalendarCreatedEvent message)
    {
        try
        {
            _logger.LogInformation(
                "--> Consume created event for CalendarCreatedEventConsumer - Event {EventId}",
                message.Id);

            _processorDbContext.Reminders.Add(new Reminder
            {
                ReminderType = message.Type,
                RemindFor = message.CalendarId.ToString(),
                RemindDate = message.StartDate
            });
            await _processorDbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to consume created event with EventId {Id} with error {Message}.", message.Id,
                ex.Message);
            throw;
        }
    }
}
