using EFCore.BulkExtensions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
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
            case UserRoles.Student:
                var students = users.Select(x => new Student
                {
                    Id = x.UserCode,
                    FullName = x.UserName,
                    MajorId = x.MajorId,
                    CapstoneId = x.CapstoneId,
                    CampusId = x.CampusId,
                    Email = x.Email,
                    IsEligible = true,
                    Status = StudentStatus.InProgress,
                    CreatedBy = context.Message.CreatedBy,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                }).ToList();

                await _dbContext.BulkInsertAsync(students);

                _logger.LogInformation("Sync attempt {Times}", context.Message.AttempTime);
                break;

            case UserRoles.Supervisor:
                var supervisors = users.Select(x => new Supervisor
                {
                    Id = x.UserCode,
                    FullName = x.UserName,
                    MajorId = x.MajorId,
                    CampusId = x.CampusId,
                    Email = x.Email,
                    CreatedBy = context.Message.CreatedBy,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                }).ToList();

                await _dbContext.BulkInsertAsync(supervisors);

                _logger.LogInformation("Sync attempt {Times}", context.Message.AttempTime);
                break;
        }
    }
}
