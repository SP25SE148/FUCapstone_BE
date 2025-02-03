using FUC.Common.Contracts;
using FUC.Data.Data;
using FUC.Data.Entities;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Consumers;
public class UsersSyncMessageConsumer : IConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly FucDbContext _dbContext;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _dbContext = serviceProvider.GetRequiredService<FucDbContext>();
    }

    public async Task Consume(ConsumeContext<UsersSyncMessage> context)
    {
        _logger.LogInformation("--> Consume users sync: {MessageId}", context.MessageId);

        var users = context.Message.UsersSync;

        switch (context.Message.UserType)
        {
            case "Student":
                // TODO: Bulk insert Student
                var students = users.Select(x => new Student
                {
                    
                }).ToList();
                break;

            case "Supervisor":
                // TODO: Bulk insert Supervisor
                break;
        } 

        foreach (var user in users)
        {
            _logger.LogInformation("{User} has synced", user.UserCode);
        }
    }
}
