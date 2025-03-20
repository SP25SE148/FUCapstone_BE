using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Hubs;
using FUC.Processor.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class UsersSyncMessageConsumer : BaseEventConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger, 
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _emailService = emailService;
        _hub = hub;
    }

    protected override async Task ProcessMessage(UsersSyncMessage message)
    {
        _logger.LogInformation("--> users sync consume in Notification service");

        var userEmails = message.UsersSync.Select(x => x.Email).ToArray();

        await _emailService.SendMailAsync("Welcome-FUC", "You can connect to FUC", userEmails);
    }
}
