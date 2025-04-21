using System.Collections.Concurrent;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Data.Entities;
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
    private readonly ILogger<ProcessRemindersJob> _logger;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;
    private readonly UsersTracker _usersTracker;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ProcessRemindersJob(ILogger<ProcessRemindersJob> logger,
        UsersTracker usersTracker,
        IServiceProvider serviceProvider,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _usersTracker = usersTracker;
        _hub = serviceProvider.GetRequiredService<IHubContext<NotificationHub, INotificationClient>>();
        _emailService = serviceProvider.GetRequiredService<IEmailService>();
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting ProcessReminderJob at {Date}.", DateTime.Now);

        var tasks = new List<Task>
        {
            ExecuteRemindersAsync(context.CancellationToken),
            ExecuteRecurrentRemindersAsync(context.CancellationToken)
        };

        await Task.WhenAll(tasks);

        _logger.LogInformation("Completed ProcessReminderJob at {Date}.", DateTime.Now);
    }

    private async Task ExecuteRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var _processorDbContext = scope.ServiceProvider.GetRequiredService<ProcessorDbContext>();
        using var integrationEventLogService = new IntegrationEventLogService<ProcessorDbContext>(_processorDbContext);

        try
        {
            _logger.LogInformation("--> Starting execute the ReminderJob at {Date}.", DateTime.Now);

            await _processorDbContext.Database.BeginTransactionAsync(cancellationToken);

            var now = DateTime.Now;

            var reminders = await _processorDbContext.Reminders
                .Where(r => r.RemindDate <= now)
                .Take(100)
                .ToListAsync(cancellationToken);

            if (reminders.Count == 0)
            {
                _logger.LogInformation("No reminders to process.");
                return;
            }

            var reminderedQueue = new ConcurrentQueue<Guid>();

            foreach (var reminder in reminders)
            {
                await ProcessReminderAsync(reminder, reminderedQueue, _processorDbContext, integrationEventLogService,
                    cancellationToken);
            }

            if (!reminderedQueue.IsEmpty)
            {
                await _processorDbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                                DELETE FROM "Reminders" WHERE "Id" = ANY({reminderedQueue.ToArray()});
                                                                                """,
                    cancellationToken: cancellationToken);
            }

            await _processorDbContext.Database.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Completed processing {Count} reminders.", reminderedQueue.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to processing reminders with Error {Message}.", ex.Message);
            await _processorDbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }

    private async Task ExecuteRecurrentRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var _processorDbContext = scope.ServiceProvider.GetRequiredService<ProcessorDbContext>();

        try
        {
            _logger.LogInformation("--> Starting execute the ReminderJob at {Date}.", DateTime.Now);

            await _processorDbContext.Database.BeginTransactionAsync(cancellationToken);

            var currentTime = DateTime.Now.TimeOfDay;
            var buffer = TimeSpan.FromMinutes(1);

            var recurrentReminders = await _processorDbContext.RecurrentReminders
                .Where(r => r.RecurringDay == DateTime.Now.DayOfWeek &&
                            r.RemindTime >= currentTime.Subtract(buffer) &&
                            r.RemindTime <= currentTime.Add(buffer))
                .Take(100)
                .ToListAsync(cancellationToken);

            if (recurrentReminders.Count == 0)
            {
                _logger.LogInformation("No recurrentReminders to process.");
                return;
            }

            var reminderedQueue = new List<Guid>();

            foreach (var reminder in recurrentReminders)
            {
                await ProcessRecurrentReminderAsync(reminder, reminderedQueue, _processorDbContext, cancellationToken);
            }

            if (reminderedQueue.Count < 1)
            {
                await _processorDbContext.Database.ExecuteSqlInterpolatedAsync($"""
                                                                                DELETE FROM "RecurrentReminders" WHERE "Id" = ANY({reminderedQueue.ToArray()});
                                                                                """,
                    cancellationToken: cancellationToken);
            }

            await _processorDbContext.Database.CommitTransactionAsync(cancellationToken);

            _logger.LogInformation("Completed processing {Count} reminders.", recurrentReminders.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to processing reminders with Error {Message}.", ex.Message);
            await _processorDbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }

    private async Task ProcessReminderAsync(Reminder reminder,
        ConcurrentQueue<Guid> reminderedQueue,
        ProcessorDbContext processorDbContext,
        IIntegrationEventLogService integrationEventLogService,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing reminder ID: {Id}, Type: {Type}", reminder.Id, reminder.ReminderType);

        try
        {
            switch (reminder.ReminderType)
            {
                case "TopicRequest":
                    integrationEventLogService.SendEvent(new TopicRequestExpirationEvent
                    {
                        TopicRequestId = Guid.Parse(reminder.RemindFor)
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    break;

                case "JoinGroupRequest":
                    integrationEventLogService.SendEvent(new JoinGroupRequestExpirationEvent
                    {
                        JoinGroupRequestId = Guid.Parse(reminder.RemindFor)
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    break;

                case "GroupMember":
                    integrationEventLogService.SendEvent(new GroupMemberExpirationEvent
                    {
                        GroupMemberId = Guid.Parse(reminder.RemindFor)
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    break;
                case nameof(ReviewCalendarExpirationEvent):
                    integrationEventLogService.SendEvent(new ReviewCalendarExpirationEvent
                    {
                        ReviewCalendarId = Guid.Parse(reminder.RemindFor)
                    });
                    break;
                case nameof(FucTaskCreatedEvent):
                    var target = reminder.Content!.Split("/");

                    var content = $"You have to done the Task '{target[^1]}' ontime.";

                    processorDbContext.Notifications.Add(new Notification
                    {
                        UserCode = reminder.RemindFor,
                        IsRead = false,
                        Content = content,
                        Type = reminder.ReminderType,
                        ReferenceTarget = reminder.Content,
                        CreatedDate = DateTime.Now,
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    var userTarget = await processorDbContext.Users
                        .FirstAsync(x => x.UserCode == reminder.RemindFor, cancellationToken);

                    ArgumentNullException.ThrowIfNull(userTarget);

                    if (!await _emailService.SendMailAsync("[FUC_TASK_REMINDER]", content, userTarget.Email))
                        throw new InvalidOperationException("Fail to send email");

                    var connections = await _usersTracker.GetConnectionForUser(reminder.RemindFor);

                    // when get this ReceiveNewNotification then fe +1 for the belt
                    await _hub.Clients.Clients(connections).ReceiveNewNotification(content);

                    break;
                //flow 1
                case nameof(TeamUpTimeConfigurationCreatedEvent.TeamUpDate):
                case nameof(TeamUpTimeConfigurationCreatedEvent.TeamUpExpirationDate):
                //flow 2
                case nameof(RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicDate):
                case nameof(RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicExpiredDate):
                //flow 3
                case nameof(RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupDate):
                case nameof(RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupExpiredDate):
                // flow 5
                case nameof(ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptDate):
                case nameof(ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptExpiredDate):
                // flow 6
                case nameof(DefendCapstoneProjectTimeConfigurationCreatedEvent.DefendCapstoneProjectDate):
                case nameof(DefendCapstoneProjectTimeConfigurationCreatedEvent.DefendCapstoneProjectExpiredDate):

                    var user = new List<User>();
                    var campusId = reminder.RemindFor.Split("/")[2 - 1];
                    if (reminder.RemindFor == "students")
                    {
                        user = await processorDbContext.Users
                            .Where(u => u.Role == UserRoles.Student && u.CampusId == campusId)
                            .ToListAsync(cancellationToken);
                    }
                    else if (reminder.RemindFor == "supervisors")
                    {
                        user = await processorDbContext.Users
                            .Where(u => u.Role == UserRoles.Supervisor && u.CampusId == campusId)
                            .ToListAsync(cancellationToken);
                    }

                    await _emailService.SendMailAsync("[FUC_Notification]",
                        reminder.Content,
                        user.Select(x => x.Email).ToArray());
                    break;


                case nameof(ReviewCalendar):
                case nameof(DefendCapstoneProjectInformationCalendar):
                    _logger.LogInformation("Processing reminder ID: {Id}, Type: {Type}", reminder.Id,
                        reminder.ReminderType);
                    if (reminder.RemindDate.Date >= DateTime.Now.Date)
                    {
                        integrationEventLogService.SendEvent(new CalendarOnTimeEvent
                        {
                            CalendarId = Guid.Parse(reminder.RemindFor),
                            Type = reminder.ReminderType,
                            Status = reminder.RemindDate.Date == DateTime.Now.Date
                                ? "InProgress"
                                : "Done"
                        });
                        await processorDbContext.SaveChangesAsync(cancellationToken);
                    }

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

    private async Task ProcessRecurrentReminderAsync(RecurrentReminder recurrentReminder,
        List<Guid> reminderedQueue,
        ProcessorDbContext processorDbContext,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing recurrent reminder ID: {Id}, Type: {Type}",
            recurrentReminder.Id,
            recurrentReminder.ReminderType);

        try
        {
            switch (recurrentReminder.ReminderType)
            {
                case nameof(ProjectProgressCreatedEvent):
                    var content = $"You have to meeting with your supervisor today.";

                    processorDbContext.Notifications.Add(new Notification
                    {
                        UserCode = recurrentReminder.RemindFor,
                        IsRead = false,
                        Content = content,
                        Type = recurrentReminder.ReminderType,
                        ReferenceTarget = recurrentReminder.Content,
                        CreatedDate = DateTime.Now,
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    var userTarget = await processorDbContext.Users
                        .FirstAsync(x => x.UserCode == recurrentReminder.RemindFor, cancellationToken);

                    ArgumentNullException.ThrowIfNull(userTarget);

                    if (!await _emailService.SendMailAsync("[FUC_MEETING_PROJECT_PROGRESS]", content, userTarget.Email))
                        throw new InvalidOperationException("Fail to send email");

                    var connections = await _usersTracker.GetConnectionForUser(recurrentReminder.RemindFor);

                    await _hub.Clients.Clients(connections).ReceiveNewNotification(content);
                    break;

                case "EvaluationProgress":
                    processorDbContext.Notifications.Add(new Notification
                    {
                        UserCode = recurrentReminder.RemindFor,
                        IsRead = false,
                        Content = $"You have to done weekly evaluation today.",
                        Type = recurrentReminder.ReminderType,
                        ReferenceTarget = recurrentReminder.Content,
                        CreatedDate = DateTime.Now,
                    });

                    await processorDbContext.SaveChangesAsync(cancellationToken);

                    var supervisorTarget = await processorDbContext.Users
                        .FirstAsync(x => x.UserCode == recurrentReminder.RemindFor, cancellationToken);

                    ArgumentNullException.ThrowIfNull(supervisorTarget);

                    if (!await _emailService.SendMailAsync("[FUC_WEEKLY_EVALUATION]",
                            $"You have to done weekly evaluation today.",
                            supervisorTarget.Email))
                        throw new InvalidOperationException("Fail to send email.");

                    var supervisorConnections = await _usersTracker.GetConnectionForUser(recurrentReminder.RemindFor);

                    await _hub.Clients.Clients(supervisorConnections)
                        .ReceiveNewNotification($"You have to done weekly evaluation today.");

                    break;

                case "GroupEvaluationProgress":
                    var studentCodes = recurrentReminder.RemindFor.Split("/");

                    var studentsConnections = new List<string>();

                    foreach (var studentCode in studentCodes)
                    {
                        processorDbContext.Notifications.Add(new Notification
                        {
                            UserCode = recurrentReminder.RemindFor,
                            IsRead = false,
                            Content = $"Your group need to done your own tasks, then evaluation for the progress week.",
                            Type = recurrentReminder.ReminderType,
                            ReferenceTarget = recurrentReminder.Content,
                            CreatedDate = DateTime.Now,
                        });

                        studentsConnections.AddRange(
                            await _usersTracker.GetConnectionForUser(recurrentReminder.RemindFor));
                    }

                    var studentEmails = await processorDbContext.Users
                        .Where(x => studentCodes.Contains(x.UserCode))
                        .Select(x => x.Email)
                        .ToArrayAsync(cancellationToken);

                    ArgumentNullException.ThrowIfNull(studentEmails);

                    if (!await _emailService.SendMailAsync("[FUC_WEEKLY_EVALUATION]",
                            $"You have to done weekly evaluation today.",
                            studentEmails))
                        throw new InvalidOperationException("Fail to send email.");

                    await _hub.Clients.Clients(studentsConnections)
                        .ReceiveNewNotification($"You have to done weekly summary evaluation today.");

                    break;

                case "Test":
                    _logger.LogInformation("Reminder test task {TaskNumber}", recurrentReminder.Content);
                    break;

                default:
                    _logger.LogWarning("Unsupported RemindType: {Type}", recurrentReminder.ReminderType);
                    break;
            }

            if (recurrentReminder.EndDate.HasValue &&
                recurrentReminder.EndDate.Value.Date - DateTime.Now.Date <= TimeSpan.FromDays(7))
                reminderedQueue.Add(recurrentReminder.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Reminder {Id} was processed fail. {Error}",
                recurrentReminder.Id,
                ex.Message);
            throw;
        }
    }
}
