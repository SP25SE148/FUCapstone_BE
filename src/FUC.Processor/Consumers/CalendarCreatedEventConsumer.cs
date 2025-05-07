using FUC.Common.Abstractions;
using FUC.Common.Constants;
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

public class CalendarCreatedEventConsumer : BaseEventConsumer<CalendarCreatedEvent>
{
    private readonly ProcessorDbContext _processorDbContext;
    private readonly ILogger<CalendarCreatedEventConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly UsersTracker _usersTracker;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public CalendarCreatedEventConsumer(
        ILogger<CalendarCreatedEventConsumer> logger,
        ProcessorDbContext processorDb,
        UsersTracker usersTracker,
        IServiceProvider serviceProvider,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = serviceProvider.GetRequiredService<IHubContext<NotificationHub, INotificationClient>>();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
        _processorDbContext = processorDb;
    }

    protected override async Task ProcessMessage(CalendarCreatedEvent message)
    {
        try
        {
            _logger.LogInformation(
                "--> Consume created event for CalendarCreatedEventConsumer - Event {EventId}",
                message.Id);

            foreach (var calendarCreatedDetail in message.Details)
            {
                foreach (string user in calendarCreatedDetail.Users)
                {
                    var supervisorsConnections = new List<string>();


                    _processorDbContext.Notifications.Add(new Notification
                    {
                        UserCode = user,
                        IsRead = false,
                        Content = $"You have new {calendarCreatedDetail.Type} created by manager.",
                        Type = calendarCreatedDetail.Type,
                        CreatedDate = DateTime.Now,
                    });

                    supervisorsConnections.AddRange(
                        await _usersTracker.GetConnectionForUser(user));

                    var supervisorEmails = await _processorDbContext.Users
                        .Where(x => calendarCreatedDetail.Users.Contains(x.UserCode))
                        .Select(x => x.Email)
                        .ToArrayAsync();

                    ArgumentNullException.ThrowIfNull(supervisorEmails);

                    if (!await _emailService.SendMailAsync($"[FUC_{calendarCreatedDetail.Type}]",
                            $"You have new {calendarCreatedDetail.Type} created, please log in to FUC to check detail.",
                            supervisorEmails))
                        throw new InvalidOperationException("Fail to send email.");

                    await _hub.Clients.Clients(supervisorsConnections)
                        .ReceiveNewNotification($"You have new {calendarCreatedDetail.Type} created by manager.");

                    _processorDbContext.Reminders.Add(new Reminder
                    {
                        ReminderType = calendarCreatedDetail.Type,
                        RemindFor = user,
                        RemindDate = calendarCreatedDetail.StartDate.AddDays(-2).StartOfDay()
                    });
                }

                _processorDbContext.Reminders.Add(new Reminder
                {
                    ReminderType = calendarCreatedDetail.Type,
                    RemindFor = calendarCreatedDetail.CalendarId.ToString(),
                    RemindDate = calendarCreatedDetail.StartDate.StartOfDay()
                });
                _processorDbContext.Reminders.Add(new Reminder
                {
                    ReminderType = calendarCreatedDetail.Type,
                    RemindFor = calendarCreatedDetail.CalendarId.ToString(),
                    RemindDate = calendarCreatedDetail.StartDate.AddDays(1).StartOfDay()
                });
            }

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
