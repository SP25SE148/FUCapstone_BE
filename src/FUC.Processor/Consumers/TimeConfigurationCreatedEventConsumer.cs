using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Helpers;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Models;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class TimeConfigurationCreatedEventConsumer : BaseEventConsumer<TimeConfigurationCreatedEvent>
{
    private readonly ILogger<TimeConfigurationCreatedEventConsumer> _logger;
    private readonly ProcessorDbContext _processorDb;

    public TimeConfigurationCreatedEventConsumer(
        ILogger<TimeConfigurationCreatedEventConsumer> logger,
        ProcessorDbContext processorDb,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _processorDb = processorDb;
    }

    protected override async Task ProcessMessage(TimeConfigurationCreatedEvent message)
    {
        try
        {
            _logger.LogInformation("--> Consume time configuration created with Id {Id} - EventId {EventId}",
                message.RequestId, message.Id);

            var reminderList = new List<Reminder>
            {
                // flow 1
                new Reminder
                {
                    RemindFor = message.TeamUpTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.TeamUpTimeConfigurationCreatedEvent.TeamUpDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.TeamUpTimeConfigurationCreatedEvent.TeamUpDate),
                    Content =
                        $"Kính gửi sinh viên,\nFUC thông báo với bạn rằng thời gian đăng ký nhóm mà bạn đã chờ đợi đã đến! Thời gian đăng ký nhóm bắt đầu diễn ra vào ngày {message.TeamUpTimeConfigurationCreatedEvent.TeamUpDate}.\n"
                },
                new Reminder
                {
                    RemindFor = message.TeamUpTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.TeamUpTimeConfigurationCreatedEvent.TeamUpExpirationDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.TeamUpTimeConfigurationCreatedEvent.TeamUpExpirationDate),
                    Content =
                        $"Kính gửi sinh viên,\nFUC thông báo với bạn rằng thời gian đăng ký nhóm đã sắp hết hạn, mau chóng tìm nhóm phù hợp để tham gia kỳ đồ án! Thời gian đăng ký nhóm sẽ hết hạn vào ngày {message.TeamUpTimeConfigurationCreatedEvent.TeamUpExpirationDate}.\n Sinh viên đã có nhóm xin vui lòng bỏ qua thông báo này!"
                },

                // flow 2
                new Reminder
                {
                    RemindFor = message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType =
                        nameof(message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicDate),
                    Content =
                        $"Kính gửi giảng viên,\nFUC thông báo với bạn rằng thời gian nộp đăng ký đề tài đã sắp đến! Thời gian nộp đăng ký đề tài bắt đầu diễn ra vào ngày {message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicDate}.\n"
                },
                new Reminder
                {
                    RemindFor = message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicExpiredDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.RegistTopicForSupervisorTimeConfigurationCreatedEvent
                        .RegistTopicExpiredDate),
                    Content =
                        $"Kính gửi giảng viên,\nFUC thông báo với bạn rằng thời gian nộp đăng ký đề tài đã sắp hết hạn, mau chóng nộp file đăng ký đề tài! Thời gian đăng ký đề tài sẽ hết hạn vào ngày {message.RegistTopicForSupervisorTimeConfigurationCreatedEvent.RegistTopicExpiredDate}.\n Giảng viên đã nộp file đăng ký đề tài đầy đủ xin vui lòng bỏ qua thông báo này!"
                },

                // flow 3
                new Reminder
                {
                    RemindFor = message.RegistTopicForGroupTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.RegistTopicForGroupTimeConfigurationCreatedEvent
                        .RegistTopicForGroupDate),
                    Content =
                        $"Kính gửi sinh viên,\nFUC thông báo với bạn rằng thời gian đăng ký đề tài cho nhóm đã sắp đến! Thời gian đăng ký đề tài cho nhóm bắt đầu diễn ra vào ngày {message.RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupDate}.\n"
                },
                new Reminder
                {
                    RemindFor = message.RegistTopicForGroupTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupExpiredDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.RegistTopicForGroupTimeConfigurationCreatedEvent
                        .RegistTopicForGroupExpiredDate),
                    Content =
                        $"Kính gửi sinh viên,\nFUC thông báo với bạn rằng thời gian đăng ký đề tài cho nhóm đã sắp hết hạn, mau chóng đăng ký đề tài! Thời gian đăng ký đề tài cho nhóm sẽ hết hạn vào ngày {message.RegistTopicForGroupTimeConfigurationCreatedEvent.RegistTopicForGroupExpiredDate}.\n Sinh viên đã nộp file đăng ký đề tài đầy đủ xin vui lòng bỏ qua thông báo này!"
                },

                // flow 5
                new Reminder
                {
                    RemindFor = message.ReviewAttemptTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptDate),
                    Content =
                        $"Kính gửi manager,\nFUC thông báo với bạn rằng thời gian import lịch các đợt review cho các nhóm đã sắp đến! Thời gian import các đợt review cho các nhóm bắt đầu diễn ra vào ngày {message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptDate}.\n"
                },
                new Reminder
                {
                    RemindFor = message.ReviewAttemptTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptExpiredDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptExpiredDate),
                    Content =
                        $"Kính gửi manager,\nFUC thông báo với bạn rằng thời gian import lịch các đợt review cho các nhóm đã sắp đến hạn! Thời gian import các đợt review cho các nhóm sẽ hết hạn vào ngày {message.ReviewAttemptTimeConfigurationCreatedEvent.ReviewAttemptExpiredDate}.\n"
                },

                // flow 6
                new Reminder
                {
                    RemindFor = message.DefendCapstoneProjectTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.DefendCapstoneProjectTimeConfigurationCreatedEvent.DefendCapstoneProjectDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.DefendCapstoneProjectTimeConfigurationCreatedEvent
                        .DefendCapstoneProjectDate),
                    Content =
                        $"Kính gửi manager,\nFUC thông báo với bạn rằng thời gian import lịch bảo vệ đồ án cho các nhóm đã sắp đến! Thời gian import lịch bảo vệ đồ án cho các nhóm bắt đầu diễn ra vào ngày {message.DefendCapstoneProjectTimeConfigurationCreatedEvent.DefendCapstoneProjectDate}.\n"
                },
                new Reminder
                {
                    RemindFor = message.ReviewAttemptTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.DefendCapstoneProjectTimeConfigurationCreatedEvent
                        .DefendCapstoneProjectExpiredDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.DefendCapstoneProjectTimeConfigurationCreatedEvent
                        .DefendCapstoneProjectExpiredDate),
                    Content =
                        $"Kính gửi manager,\nFUC thông báo với bạn rằng thời gian import import lịch bảo vệ đồ án cho các nhóm đã sắp đến hạn! Thời gian import lịch bảo vệ đồ án cho các nhóm sẽ hết hạn vào ngày {message.DefendCapstoneProjectTimeConfigurationCreatedEvent.DefendCapstoneProjectExpiredDate}.\n"
                }
            };

            foreach (var reminder in reminderList)
            {
                await _processorDb.Reminders.AddAsync(reminder);
            }

            await _processorDb.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Consume time configuration created with EventId {EventId} with error {Message}.",
                message.Id, e.Message);
            throw;
        }
    }
}
