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

public class ProjectProgressCreatedEventConsumer : BaseEventConsumer<ProjectProgressCreatedEvent>
{
    private readonly ILogger<ProjectProgressCreatedEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly UsersTracker _usersTracker;
    private readonly IEmailService _emailService;   

    public ProjectProgressCreatedEventConsumer(ILogger<ProjectProgressCreatedEventConsumer> logger, 
        ProcessorDbContext processorDbContext,
        UsersTracker usersTracker,
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _emailService = emailService;
        _hub = hub;
        _processorDbContext = processorDbContext;
    }

    protected override async Task ProcessMessage(ProjectProgressCreatedEvent message)
    {
        try
        {
            _logger.LogInformation("--> Consume event for {RequestType} with GroupId {Id} - Event {EventId}",
            message.Type,
            message.ProjectProgressId,
            message.Id);

            await _processorDbContext.Database.BeginTransactionAsync();

            // remind recurrent with DayOfWeek
            var recurrentReminderForMeeting = new RecurrentReminder
            {
                ReminderType = message.Type,
                Content = $"{message.GroupId}/{message.ProjectProgressId}",
                EndDate = message.EndDate,
                RemindFor = string.Join("/", message.StudentCodes),
                RecurringDay = message.RemindDate,
                RemindTime = message.RemindTime,
            };

            var recurrentReminderForEvaluation = new RecurrentReminder
            {
                ReminderType = "EvaluationProgress",
                Content = $"{message.GroupId}/{message.ProjectProgressId}",
                EndDate = message.EndDate,
                RemindFor = message.SupervisorCode,
                RecurringDay = DayOfWeek.Saturday,
                RemindTime = TimeSpan.FromHours(10),
            };

            var recurrentReminderForGroupEvaluation = new RecurrentReminder
            {
                ReminderType = "GroupEvaluationProgress",
                Content = $"{message.GroupId}/{message.ProjectProgressId}",
                EndDate = message.EndDate,
                RemindFor = string.Join("/", message.StudentCodes),
                RecurringDay = DayOfWeek.Saturday,
                RemindTime = TimeSpan.FromHours(8),
            };

            _processorDbContext.AddRange(recurrentReminderForMeeting, 
                recurrentReminderForEvaluation, 
                recurrentReminderForGroupEvaluation);

            foreach (var student in message.StudentCodes)
            {
                _processorDbContext.Notifications.Add(new Notification
                {
                    Content = $"Supervisor {message.SupervisorName} was imported our project progress into Group.",
                    ReferenceTarget = recurrentReminderForMeeting.Content,
                    Type = message.Type,
                    IsRead = false,
                    UserCode = student,
                    CreatedDate = DateTime.Now,
                });

                await _processorDbContext.SaveChangesAsync();

                var connections = await _usersTracker.GetConnectionForUser(student);

                if (connections.Count == 0)
                {
                    var emailForUser = await _processorDbContext.Users.FirstAsync(x => x.UserCode == student);

                    if (!await _emailService.SendMailAsync("[FUC_PROJECT_PROGRESS]", 
                        $"Supervisor {message.SupervisorName} was imported our project progress into Group.", 
                        emailForUser.Email))
                        throw new InvalidOperationException("Fail to send email");
                }
                else
                {
                    await _hub.Clients.Clients(connections)
                        .ReceiveNewNotification($"Supervisor {message.SupervisorName} was imported our project progress into Group.");
                }
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
}
