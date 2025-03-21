﻿using System.Collections.Concurrent;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Processor.Data;
using FUC.Processor.Hubs;
using FUC.Processor.Models;
using FUC.Processor.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace FUC.Processor.Jobs;

[DisallowConcurrentExecution]
public class ProcessRemindersJob : IJob
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<ProcessRemindersJob> _logger;
    private readonly IIntegrationEventLogService _integrationEventLogService;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly UsersTracker _usersTracker;

    public ProcessRemindersJob(ILogger<ProcessRemindersJob> logger,
        ProcessorDbContext processorDbContext,
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub,
        UsersTracker usersTracker,
        IIntegrationEventLogService integrationEventLogService)
    {
        _processorDbContext = processorDbContext;
        _integrationEventLogService = integrationEventLogService;
        _logger = logger;
        _hub = hub;
        _usersTracker = usersTracker;
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
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

            foreach(var reminder in reminders)
            {
                await ProcessReminderAsync(reminder, reminderedQueue, context.CancellationToken);
            }

            if (!reminderedQueue.IsEmpty)
            {
                await _processorDbContext.Database.ExecuteSqlInterpolatedAsync($"""
                DELETE FROM "Reminders" WHERE "Id" = ANY({reminderedQueue.ToArray()});
                """);
            }

            await _processorDbContext.Database.CommitTransactionAsync(context.CancellationToken);

            _logger.LogInformation("Completed processing {Count} reminders.", reminderedQueue.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to processing reminders with Error {Message}.", ex.Message);
            await _processorDbContext.Database.RollbackTransactionAsync(context.CancellationToken);
        }
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

                case "RemindDueDateTask":
                    var target = reminder.Content!.Split("/");

                    var content = $"You have to done the Task {target[2]} ontime.";

                    _processorDbContext.Notifications.Add(new Notification
                    {
                        UserCode = reminder.RemindFor,
                        IsRead = false,
                        Content = content,
                        Type = reminder.ReminderType,
                        ReferenceTarget = reminder.Content,
                        CreatedDate = DateTime.Now,
                    });

                    await _processorDbContext.SaveChangesAsync(cancellationToken);

                    var userTarget = await _processorDbContext.Users
                        .FirstAsync(x => x.UserCode == reminder.RemindFor, cancellationToken);

                    ArgumentNullException.ThrowIfNull(userTarget);

                    if (!await _emailService.SendMailAsync("[FUC_TASK_REMINDER]", content, userTarget.Email))
                        throw new InvalidOperationException("Fail to send email");

                    var connections = await _usersTracker.GetConnectionForUser(reminder.RemindFor);

                    // when get this ReceiveNewNotification then fe +1 for the belt
                    await _hub.Clients.Clients(connections).ReceiveNewNotification(content);

                    break;

                case "Test":
                    _logger.LogInformation("Reminder test task {TaskNumber}", reminder.Content);
                    break;

                default:
                    _logger.LogWarning("Unsupported RemindType: {Type}", reminder.ReminderType);
                    break;
            }

            reminderedQueue.Enqueue(reminder.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Reminder {Id} was processed fail. {Error}", reminder.Id, ex.Message);
            throw;
        }
    }
}
