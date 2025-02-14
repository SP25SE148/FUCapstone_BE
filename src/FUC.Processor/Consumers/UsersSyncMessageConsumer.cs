using FUC.Common.Contracts;
using FUC.Processor.Hubs;
using FUC.Processor.Services;
using MassTransit;
using Microsoft.AspNetCore.SignalR;

namespace FUC.Processor.Consumers;

public class UsersSyncMessageConsumer : IConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly IEmailService _emailService;
    private readonly IHubContext<NotificationHub, INotificationClient> _hub;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger, 
        IEmailService emailService,
        IHubContext<NotificationHub, INotificationClient> hub)
    {
        _logger = logger;
        _emailService = emailService;
        _hub = hub;
    }

    public async Task Consume(ConsumeContext<UsersSyncMessage> context)
    {
        _logger.LogInformation("--> users sync consume in Notification service");

        var userEmails = context.Message.UsersSync.Select(x => x.Email).ToArray();

        await _emailService.SendMailAsync("Welcome-FUC", "You can connect to FUC", userEmails);
    }
}
