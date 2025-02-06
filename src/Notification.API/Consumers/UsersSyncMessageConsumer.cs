using FUC.Common.Contracts;
using MassTransit;
using Notification.API.Services;

namespace Notification.API.Consumers;

public class UsersSyncMessageConsumer : IConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly IEmailService _emailService;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger, IEmailService emailService)
    {
        _logger = logger;
        _emailService = emailService;
    }

    public async Task Consume(ConsumeContext<UsersSyncMessage> context)
    {
        _logger.LogInformation("--> users sync consume in Notification service");

        var userEmails = context.Message.UsersSync.Select(x => x.Email).ToArray();

        await _emailService.SendMailAsync("Welcome-FUC", "You can connect to FUC", userEmails);
    }
}
