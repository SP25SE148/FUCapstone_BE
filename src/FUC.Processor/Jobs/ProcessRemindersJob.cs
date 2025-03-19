using System.Collections.Concurrent;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FUC.Processor.Jobs;

public class ProcessRemindersJob : IJob
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<ProcessRemindersJob> _logger;
    private readonly IIntegrationEventLogService _integrationEventLogService;

    public ProcessRemindersJob(ILogger<ProcessRemindersJob> logger,
        ProcessorDbContext processorDbContext,
        IIntegrationEventLogService integrationEventLogService)
    {
        _processorDbContext = processorDbContext;
        _integrationEventLogService = integrationEventLogService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("--> Starting execute the ReminderJob at {Date}.", DateTime.Now);

        await _processorDbContext.Database.BeginTransactionAsync(context.CancellationToken);

        var reminders = await _processorDbContext.Reminders.FromSqlRaw(
            $"""
            SELECT * 
            FROM "Reminders"
            WHERE DATE("RemindDate") = CURRENT_DATE;
            """
            ).ToListAsync(context.CancellationToken);

        if (reminders.Count == 0)
        {
            _logger.LogInformation("No reminders to process.");
            return;
        }

        var reminderedQueue = new ConcurrentQueue<Guid>();

        var reminderTasks = reminders.Select(x => ProcessReminderAsync(x,
            reminderedQueue,
            context.CancellationToken));

        await Task.WhenAll(reminderTasks);

        if (!reminderedQueue.IsEmpty)
        {
            await _processorDbContext.Database.ExecuteSqlInterpolatedAsync($"""
                DELETE FROM "Reminders" WHERE "Id" = ANY({reminderedQueue.ToArray()});
                """);
        }

        await _processorDbContext.Database.CommitTransactionAsync(context.CancellationToken);

        _logger.LogInformation("Completed processing {Count} reminders.", reminderedQueue.Count);
    }

    private async Task ProcessReminderAsync(Reminder reminder,
        ConcurrentQueue<Guid> reminderedQueue,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing reminder ID: {Id}, Type: {Type}", reminder.Id, reminder.ReminderType);

        try
        {
            if (reminder.RemindDate > DateTime.Now)
                return;

            switch (reminder.ReminderType)
            {
                case "TopicRequest":
                    _integrationEventLogService.SendEvent(new TopicRequestExpirationEvent
                    {
                        TopicRequestId = Guid.Parse(reminder.RemindFor)
                    });

                    break;

                case "JoinGroupRequest":
                    _integrationEventLogService.SendEvent(new JoinGroupRequestExpirationEvent
                    {
                        JoinGroupRequestId = Guid.Parse(reminder.RemindFor)
                    });

                    break;

                case "GroupMember":
                    _integrationEventLogService.SendEvent(new GroupMemberExpirationEvent
                    {
                        GroupMemberId = Guid.Parse(reminder.RemindFor)
                    });

                    break;

                case "Test":
                    _logger.LogInformation("Reminder test task {TaskNumber}", reminder.Content);
                    break;

                default:
                    _logger.LogWarning("Unsupported RemindType: {Type}", reminder.ReminderType);
                    break;
            }

            await _processorDbContext.SaveChangesAsync(cancellationToken);

            reminderedQueue.Enqueue(reminder.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Reminder {Id} was processed fail. {Error}", reminder.Id, ex.Message);
        }
    }
}
