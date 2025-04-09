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

            // remind on time before
            var earlyReminderList = new List<Reminder>
            {
                // team up reminder
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
                // team up expiration reminder
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
                // regist topic reminder
                new Reminder
                {
                    RemindFor = message.RegistTopicTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicDate),
                    Content =
                        $"Kính gửi giảng viên,\nFUC thông báo với bạn rằng thời gian nộp đăng ký đề tài đã sắp đến! Thời gian nộp đăng ký đề tài bắt đầu diễn ra vào ngày {message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicDate}.\n"
                },
                // regist topic expiration reminder
                new Reminder
                {
                    RemindFor = message.RegistTopicTimeConfigurationCreatedEvent.NotificationFor,
                    RemindDate = message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicExpiredDate
                        .StartOfDay()
                        .Add(message.RemindTime)
                        .AddDays(-message.RemindInDaysBeforeDueDate),
                    ReminderType = nameof(message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicExpiredDate),
                    Content =
                        $"Kính gửi giảng viên,\nFUC thông báo với bạn rằng thời gian nộp đăng ký đề tài đã sắp hết hạn, mau chóng nộp file đăng ký đề tài! Thời gian đăng ký đề tài sẽ hết hạn vào ngày {message.RegistTopicTimeConfigurationCreatedEvent.RegistTopicExpiredDate}.\n Giảng viên đã nộp file đăng ký đề tài đầy đủ xin vui lòng bỏ qua thông báo này!"
                }
            };
            await _processorDb.Reminders.AddRangeAsync(earlyReminderList);
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
