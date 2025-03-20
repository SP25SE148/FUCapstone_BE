using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Helpers;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Hubs;
using FUC.Processor.Models;
using FUC.Processor.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class FucTaskCreatedEventConsumer : BaseEventConsumer<FucTaskCreatedEvent>
{
    private readonly ILogger<FucTaskCreatedEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly UsersTracker _usersTracker;
    private readonly IEmailService _emailService;

    public FucTaskCreatedEventConsumer(ILogger<FucTaskCreatedEventConsumer> logger, 
        ProcessorDbContext processorDbContext,
        IHubContext<NotificationHub, INotificationClient> hub,
        UsersTracker usersTracker,
        IEmailService emailService,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;   
        _processorDbContext = processorDbContext;
        _hub = hub;
        _usersTracker = usersTracker;
        _emailService = emailService;
    }

    protected override async Task ProcessMessage(FucTaskCreatedEvent message)
    {
        try
        {
            _logger.LogInformation("--> Consume task created event for {RequestType} with TaskId {Id} - Event {EventId}",
            message.ReminderType,
            message.FucTaskId,
            message.Id);

            await _processorDbContext.Database.BeginTransactionAsync();

            // remind on due date
            await AddReminderTask(new Reminder
            {
                ReminderType = message.ReminderType,
                Content = $"{message.FucTaskId}/{message.KeyTask}",
                RemindDate = DateTime.Now.StartOfDay()
                            .Add(message.RemindTimeOnDueDate),
                RemindFor = message.NotificationFor,
            });

            if (message.RemindInDaysBeforeDueDate > 0)
            {
                // remind on day befor due date
                await AddReminderTask(new Reminder
                {
                    ReminderType = message.ReminderType,
                    Content = $"{message.FucTaskId}/{message.KeyTask}",
                    RemindDate = DateTime.Now.StartOfDay()
                        .Add(message.RemindTimeOnDueDate)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    RemindFor = message.NotificationFor,
                });
            }

            var notification = new Notification
            {
                Content = $"{message.ReporterName} assigned you into Task {message.KeyTask}.",
                ReferenceTarget = $"{message.FucTaskId}/{message.KeyTask}",
                Type = message.ReminderType,
                IsRead = false,
                UserCode = message.NotificationFor,
                CreatedDate = DateTime.Now,
            };

            _processorDbContext.Notifications.Add(notification);
            await _processorDbContext.SaveChangesAsync();

            var connections = await _usersTracker.GetConnectionForUser(message.NotificationFor);

            if (connections.Count == 0)
            {
                var emailForUser = await _processorDbContext.Users.FirstAsync(x => x.UserCode == message.NotificationFor);

                await _emailService.SendMailAsync("[FUC_TASK]", $"{notification.Content}", emailForUser.Email);
            }
            else
            {
                await _hub.Clients.Clients(connections).ReceiveNewNotification(notification.Content);
            }

            await _processorDbContext.Database.CommitTransactionAsync();
        }
        catch (Exception ex) 
        {
            _logger.LogError("Fail to consume the created FucTask event Id {EventId} with error {Message}.", message.Id, ex.Message);
            await _processorDbContext.Database.RollbackTransactionAsync();
            throw;
        }
    }

    private async Task AddReminderTask(Reminder reminder)
    {
        _processorDbContext.Reminders.Add(reminder);

        await _processorDbContext.SaveChangesAsync();
    }
}
