using AutoMapper;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.ConfigDTO;
using FUC.Service.DTOs.SemesterDTO;
using Microsoft.EntityFrameworkCore;

namespace FUC.Service.Services;

public class TimeConfigurationService(
    ICurrentUser currentUser,
    IMapper mapper,
    IRepository<TimeConfiguration> repository,
    IRepository<Semester> semesterRepository,
    ISemesterService semesterService,
    ISystemConfigurationService systemConfigurationService,
    IIntegrationEventLogService integrationEventLogService,
    IUnitOfWork<FucDbContext> unitOfWork) : ITimeConfigurationService
{
    public async Task<OperationResult<IList<TimeConfigurationDto>>> GetTimeConfigurations()
    {
        var result = await repository.FindAsync(
            x => currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId,
            include: x => x.Include(x => x.Semester),
            orderBy: null,
            selector: x => new TimeConfigurationDto
            {
                Id = x.Id,
                SemesterId = x.Semester.Id,
                SemesterName = x.Semester.Name,
                CampusId = x.CampusId,
                RegistTopicForSupervisorDate = x.RegistTopicForSupervisorDate,
                RegistTopicForSupervisorExpiredDate = x.RegistTopicForSupervisorExpiredDate,
                TeamUpDate = x.TeamUpDate,
                TeamUpExpirationDate = x.TeamUpExpirationDate,
                RegistTopicForGroupDate = x.RegistTopicForGroupDate,
                RegistTopicForGroupExpiredDate = x.RegistTopicForGroupExpiredDate,
                ReviewAttemptDate = x.ReviewAttemptDate,
                ReviewAttemptExpiredDate = x.ReviewAttemptExpiredDate,
                DefendCapstoneProjectDate = x.DefendCapstoneProjectDate,
                DefendCapstoneProjectExpiredDate = x.DefendCapstoneProjectExpiredDate,
                IsActived = x.IsActived
            });

        return OperationResult.Success(result);
    }

    public async Task<OperationResult<TimeConfigurationDto>> GetTimeConfigurationBySemesterId(string semesterId)
    {
        var result =
            from tc in repository.GetQueryable()
            join s in semesterRepository.GetQueryable() on tc.Id equals s.TimeConfigurationId
            where s.Id == semesterId && (currentUser.CampusId == "all" || tc.CampusId == currentUser.CampusId)
            select new TimeConfigurationDto
            {
                Id = tc.Id,
                SemesterId = tc.Semester.Id,
                SemesterName = tc.Semester.Name,
                CampusId = tc.CampusId,
                RegistTopicForSupervisorDate = tc.RegistTopicForSupervisorDate,
                RegistTopicForSupervisorExpiredDate = tc.RegistTopicForSupervisorExpiredDate,
                TeamUpDate = tc.TeamUpDate,
                TeamUpExpirationDate = tc.TeamUpExpirationDate,
                RegistTopicForGroupDate = tc.RegistTopicForGroupDate,
                RegistTopicForGroupExpiredDate = tc.RegistTopicForGroupExpiredDate,
                ReviewAttemptDate = tc.ReviewAttemptDate,
                ReviewAttemptExpiredDate = tc.ReviewAttemptExpiredDate,
                DefendCapstoneProjectDate = tc.DefendCapstoneProjectDate,
                DefendCapstoneProjectExpiredDate = tc.DefendCapstoneProjectExpiredDate,
                IsActived = tc.IsActived
            };
        return await result.FirstOrDefaultAsync();
    }

    public async Task<OperationResult> UpdateTimeConfiguration(
        UpdateTimeConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure(currentSemester.Error);

        var timeConfig = await repository.GetAsync(
            x => x.Id == request.Id,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(timeConfig);

        var invalidDates = new[]
        {
            (request.TeamUpDate, "TeamUpDate", "TeamUp"),
            (request.TeamUpExpirationDate, "TeamUpExpirationDate", "TeamUp"),
            (request.RegistTopicForSupervisorDate, "RegistTopicForSupervisorDate", "RegistTopicForSupervisor"),
            (request.RegistTopicForSupervisorExpiredDate, "RegistTopicForSupervisorExpiredDate",
                "RegistTopicForSupervisor"),
            (request.RegistTopicForGroupDate, "RegistTopicForGroupDate", "RegistTopicForGroup"),
            (request.RegistTopicForGroupExpiredDate, "RegistTopicForGroupExpiredDate", "RegistTopicForGroup"),
            (request.ReviewAttemptDate, "ReviewAttemptDate", "ReviewAttempt"),
            (request.ReviewAttemptExpiredDate, "ReviewAttemptExpiredDate", "ReviewAttempt"),
            (request.DefendCapstoneProjectDate, "DefendCapstoneProjectDate", "DefendCapstoneProject"),
            (request.DefendCapstoneProjectExpiredDate, "DefendCapstoneProjectExpiredDate", "DefendCapstoneProject"),
        };

        foreach (var (date, dateName, configType) in invalidDates)
        {
            if (date.HasValue && !CheckConfigurationDateIsValid(date.Value, currentSemester.Value, configType))
                return OperationResult.Failure(new Error("TimeConfiguration.Error",
                    $"{dateName} need to be in the valid duration of Semester {currentSemester.Value.Id}"));
        }

        mapper.Map(request, timeConfig);

        repository.Update(timeConfig);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult> CreateTimeConfiguration(CreateTimeConfigurationRequest request,
        CancellationToken cancellationToken)
    {
        var nextSemester = await semesterService.GetNextSemesterAsync(true);

        if (nextSemester.IsFailure)
            return OperationResult.Failure(nextSemester.Error);

        var invalidDates = new[]
        {
            (request.TeamUpDate, "TeamUpDate", "TeamUp"),
            (request.TeamUpExpirationDate, "TeamUpExpirationDate", "TeamUp"),
            (request.RegistTopicForSupervisorDate, "RegistTopicForSupervisorDate", "RegistTopicForSupervisor"),
            (request.RegistTopicForSupervisorExpiredDate, "RegistTopicForSupervisorExpiredDate",
                "RegistTopicForSupervisor"),
            (request.RegistTopicForGroupDate, "RegistTopicForGroupDate", "RegistTopicForGroup"),
            (request.RegistTopicForGroupExpiredDate, "RegistTopicForGroupExpiredDate", "RegistTopicForGroup"),
            (request.ReviewAttemptDate, "ReviewAttemptDate", "ReviewAttempt"),
            (request.ReviewAttemptExpiredDate, "ReviewAttemptExpiredDate", "ReviewAttempt"),
            (request.DefendCapstoneProjectDate, "DefendCapstoneProjectDate", "DefendCapstoneProject"),
            (request.DefendCapstoneProjectExpiredDate, "DefendCapstoneProjectExpiredDate", "DefendCapstoneProject"),
        };

        foreach (var (date, dateName, configType) in invalidDates)
        {
            if (!CheckConfigurationDateIsValid(date, nextSemester.Value, configType))
                return OperationResult.Failure(new Error("TimeConfiguration.Error",
                    $"{dateName} need to be in the valid duration of Semester {nextSemester.Value.Id}"));
        }

        if (string.IsNullOrEmpty(request.CampusId))
        {
            request.CampusId = currentUser.CampusId;
        }

        var timeConfig = new TimeConfiguration
        {
            Id = Guid.NewGuid(),
            RegistTopicForSupervisorDate = request.RegistTopicForSupervisorDate,
            RegistTopicForSupervisorExpiredDate = request.RegistTopicForSupervisorExpiredDate,
            RegistTopicForGroupDate = request.RegistTopicForGroupDate,
            RegistTopicForGroupExpiredDate = request.RegistTopicForGroupExpiredDate,
            ReviewAttemptDate = request.ReviewAttemptDate,
            ReviewAttemptExpiredDate = request.ReviewAttemptExpiredDate,
            DefendCapstoneProjectDate = request.DefendCapstoneProjectDate,
            DefendCapstoneProjectExpiredDate = request.DefendCapstoneProjectExpiredDate,
            TeamUpDate = request.TeamUpDate,
            TeamUpExpirationDate = request.TeamUpExpirationDate,
            IsActived = request.IsActived,
            CampusId = currentUser.Role == UserRoles.SuperAdmin ? request.CampusId : currentUser.CampusId,
        };

        repository.Insert(timeConfig);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        nextSemester.Value.TimeConfigurationId = timeConfig.Id;
        integrationEventLogService.SendEvent(new TimeConfigurationCreatedEvent()
        {
            RequestId = timeConfig.Id,
            CampusId = currentUser.CampusId,
            RemindInDaysBeforeDueDate = systemConfigurationService.GetSystemConfiguration()
                .TimeConfigurationRemindInDaysBeforeDueDate,
            RemindTime = TimeSpan.FromHours(7),
            RegistTopicForSupervisorTimeConfigurationCreatedEvent =
                new RegistTopicForSupervisorTimeConfigurationCreatedEvent
                {
                    NotificationFor = $"supervisors/{currentUser.CampusId}",
                    RegistTopicDate = timeConfig.RegistTopicForSupervisorDate,
                    RegistTopicExpiredDate = timeConfig.RegistTopicForSupervisorExpiredDate
                },
            TeamUpTimeConfigurationCreatedEvent = new TeamUpTimeConfigurationCreatedEvent
            {
                NotificationFor = $"students/{currentUser.CampusId}",
                TeamUpDate = timeConfig.TeamUpDate,
                TeamUpExpirationDate = timeConfig.TeamUpExpirationDate
            },
            RegistTopicForGroupTimeConfigurationCreatedEvent = new RegistTopicForGroupTimeConfigurationCreatedEvent
            {
                NotificationFor = $"students/{currentUser.CampusId}",
                RegistTopicForGroupDate = timeConfig.RegistTopicForGroupDate,
                RegistTopicForGroupExpiredDate = timeConfig.RegistTopicForGroupExpiredDate
            },
            ReviewAttemptTimeConfigurationCreatedEvent = new ReviewAttemptTimeConfigurationCreatedEvent
            {
                NotificationFor = $"supervisors/{currentUser.CampusId}",
                ReviewAttemptDate = timeConfig.ReviewAttemptDate,
                ReviewAttemptExpiredDate = timeConfig.ReviewAttemptExpiredDate
            },
            DefendCapstoneProjectTimeConfigurationCreatedEvent = new DefendCapstoneProjectTimeConfigurationCreatedEvent
            {
                NotificationFor = $"supervisors/{currentUser.CampusId}",
                DefendCapstoneProjectDate = timeConfig.DefendCapstoneProjectDate,
                DefendCapstoneProjectExpiredDate = timeConfig.DefendCapstoneProjectExpiredDate
            }
        });
        await unitOfWork.SaveChangesAsync(cancellationToken);


        return OperationResult.Success();
    }

    private static bool CheckConfigurationDateIsValid(DateTime configDate, Semester currentSemester, string configType)
    {
        return configType switch
        {
            "RegistTopicForSupervisor" => configDate <= currentSemester.StartDate,
            "DefendCapstoneProject" => configDate >= currentSemester.EndDate,
            _ => configDate >= currentSemester.StartDate && configDate <= currentSemester.EndDate,
        };
    }
}
