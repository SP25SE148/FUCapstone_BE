using EFCore.BulkExtensions;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Consumers;
public class UsersSyncMessageConsumer : BaseEventConsumer<UsersSyncMessage>
{
    private readonly ILogger<UsersSyncMessageConsumer> _logger;
    private readonly FucDbContext _dbContext;

    public UsersSyncMessageConsumer(ILogger<UsersSyncMessageConsumer> logger, IServiceProvider serviceProvider) : base(logger)
    {
        _logger = logger;
        _dbContext = serviceProvider.GetRequiredService<FucDbContext>();
    }

    protected override async Task ProcessMessage(UsersSyncMessage message)
    {
        _logger.LogInformation("--> Consume users sync: {MessageId}, attempt: {Times}",
            message.Id,
            message.AttempTime);

        var users = message.UsersSync;

        switch (message.UserType)
        {
            case UserRoles.Student:
                var students = users.Select(x => new Student
                {
                    Id = x.UserCode,
                    FullName = x.UserName,
                    MajorId = x.MajorId.ToUpper(),
                    CapstoneId = x.CapstoneId.ToUpper(),
                    CampusId = x.CampusId.ToUpper(),
                    Email = x.Email,
                    IsEligible = true,
                    Status = StudentStatus.InProgress,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                }).ToList();

                await _dbContext.BulkInsertAsync(students);

                _logger.LogInformation("Sync attempt {Times}", message.AttempTime);
                break;

            case UserRoles.Supervisor:
                var supervisors = users.Select(x => new Supervisor
                {
                    Id = x.UserCode,
                    FullName = x.UserName,
                    MajorId = x.MajorId.ToUpper(),
                    CampusId = x.CampusId.ToUpper(),
                    Email = x.Email,
                    CreatedBy = message.CreatedBy,
                    CreatedDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc)
                }).ToList();

                await _dbContext.BulkInsertAsync(supervisors);

                _logger.LogInformation("Sync attempt {Times}", message.AttempTime);
                break;
        }
    }
}
