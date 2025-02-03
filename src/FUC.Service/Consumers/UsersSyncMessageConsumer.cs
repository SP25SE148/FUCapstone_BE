using EFCore.BulkExtensions;
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
        _logger.LogInformation("--> Consume users sync: {MessageId}, attempt: {Times}", 
            context.MessageId, 
            context.Message.AttempTime);

        var users = context.Message.UsersSync;

        switch (context.Message.UserType)
        {
            case "Student":
                var students = users.Select(x => new Student
                {
                    Id = x.UserCode,
                    MajorId = x.MajorId,
                    CapstoneId = x.CapstoneId,
                    CampusId = x.CampusId,
                    Email = x.Email,
                    IsEligible = true,
                    CreatedBy = context.Message.CreatedBy,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                }).ToList();

                await _dbContext.BulkInsertAsync(students);

                _logger.LogInformation("Sync attempt {Times}", context.Message.AttempTime);
                break;

            case "Supervisor":
                // TODO: Bulk insert Supervisor
                break;
        }
    }
}
