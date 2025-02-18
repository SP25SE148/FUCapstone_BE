using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FUC.Processor.Jobs;

public class ProcessRemindersJob : IJob
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<ProcessRemindersJob> _logger;

    public ProcessRemindersJob(ProcessorDbContext processorDbContext, ILogger<ProcessRemindersJob> logger)
    {
        _processorDbContext = processorDbContext;   
        _logger = logger;   
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--> Starting execute the ReminderJob at {Date}.", DateTime.Now);

        var reminders = await _processorDbContext.Reminders.FromSqlRaw(
            $"""
            SELECT * 
            FROM "Reminders"
            WHERE RemindDate IN (CURRENT_DATE, CURRENT_DATE + INTERVAL 1 DAY);
            """
            ).ToListAsync(context.CancellationToken);

        if (reminders.Count == 0)
        {
            _logger.LogInformation("No reminders to process.");
            return;
        } 

        var reminderTasks = reminders.Select(x => ProcessReminderAsync(x));

        await Task.WhenAll(reminderTasks);

        _logger.LogInformation("Completed processing all reminders.");

        _logger.LogInformation("{Number} reminders", reminders.Count);
    }

    private async Task ProcessReminderAsync(Reminder reminder)
    {
        _logger.LogInformation("Processing reminder ID: {Id}, Type: {Type}", reminder.Id, reminder.ReminderType);

        switch (reminder.ReminderType)
        {
            default:
                _logger.LogWarning("Unsupported RemindType: {Type}", reminder.ReminderType);
                break;
        }
    }
}
