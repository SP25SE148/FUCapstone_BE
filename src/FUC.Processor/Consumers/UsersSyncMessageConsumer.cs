using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.Options;
using FUC.Processor.Data;
using FUC.Processor.Models;
using FUC.Processor.Services;
using Microsoft.Extensions.Options;

namespace FUC.Processor.Consumers;

public class UsersSyncMessageConsumer : BaseEventConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly ProcessorDbContext _processorDbContext;
    private readonly IEmailService _emailService;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger,
        IEmailService emailService,
        ProcessorDbContext processorDbContext,
        IOptions<EventConsumerConfiguration> options) : base(logger, options)
    {
        _logger = logger;
        _processorDbContext = processorDbContext;
        _emailService = emailService;
    }

    protected override async Task ProcessMessage(UsersSyncMessage message)
    {
        try
        {
            _logger.LogInformation("--> users sync consume in Notification service");

            await _processorDbContext.Database.BeginTransactionAsync();

            var users = message.UsersSync.Select(x => new User
            {
                UserCode = x.UserCode,
                Email = x.Email,
                Role = message.UserType,
                CampusId = x.CampusId
            }).ToList();

            await _processorDbContext.Users.AddRangeAsync(users);

            await _processorDbContext.SaveChangesAsync();

            var userEmails = message.UsersSync.Select(x => x.Email).ToArray();

            await _emailService.SendMailAsync("Welcome-FUC", "You can connect to FUC", userEmails);

            await _processorDbContext.Database.CommitTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to sync user into ProcessorService with error {Message}.", ex.Message);
            await _processorDbContext.Database.RollbackTransactionAsync();

            throw;
        }
    }
}
