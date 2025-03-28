using AutoMapper;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.ConfigDTO;

namespace FUC.Service.Services;

public class TimeConfigurationService(
    ICurrentUser currentUser,
    IMapper mapper,
    IRepository<TimeConfiguration> repository,
    ISemesterService semesterService,
    IUnitOfWork<FucDbContext> unitOfWork) : ITimeConfigurationService
{
    public async Task<OperationResult<IList<TimeConfigurationDto>>> GetTimeConfigurations()
    {
        var result = await repository.FindAsync(
            x => currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId,
            include: null,
            orderBy: null,
            selector: x => new TimeConfigurationDto
            {
                Id = x.Id,
                CampusId = x.CampusId,  
                RegistTopicDate = x.RegistTopicDate,    
                RegistTopicExpiredDate = x.RegistTopicExpiredDate,
                TeamUpDate = x.TeamUpDate,  
                TeamUpExpirationDate = x.TeamUpExpirationDate,  
                IsActived = x.IsActived
            });

        return OperationResult.Success(result);
    }

    public async Task<OperationResult<TimeConfigurationDto>> GetCurrentTimeConfiguration(string campusId)
    {
        var result = await repository.GetAsync(
            x => x.CampusId == campusId,
            selector: x => new TimeConfigurationDto
            {
                Id = x.Id,
                CampusId = x.CampusId,
                RegistTopicDate = x.RegistTopicDate,
                RegistTopicExpiredDate = x.RegistTopicExpiredDate,
                TeamUpDate = x.TeamUpDate,
                TeamUpExpirationDate = x.TeamUpExpirationDate,
                IsActived = x.IsActived 
            });

        if (result is null)
            return OperationResult.Failure<TimeConfigurationDto>(new Error("TimeConfiguration.Error", 
                "The timeconfig does not exist."));

        return OperationResult.Success(result);
    }

    public async Task<OperationResult> UpdateTimeConfiguration(UpdateTimeConfigurationRequest request, 
        CancellationToken cancellationToken)
    {
        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure(currentSemester.Error);

        var timeConfig = await repository.GetAsync(
            x => x.Id == request.Id, 
            cancellationToken);

        ArgumentNullException.ThrowIfNull(timeConfig);

        if (request.TeamUpDate.HasValue && 
            !CheckConfigurationDateIsValid(request.TeamUpDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"TimeUpDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.TeamUpExpirationDate.HasValue && 
            !CheckConfigurationDateIsValid(request.TeamUpExpirationDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"TimeUpExpirationDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.RegistTopicDate.HasValue && 
            !CheckConfigurationDateIsValid(request.RegistTopicDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"RegistTopicDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.RegistTopicExpiredDate.HasValue && 
            !CheckConfigurationDateIsValid(request.RegistTopicExpiredDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"RegistTopicExpiredDate need to in the duration of Semester {currentSemester.Value.Id}"));

        mapper.Map(request, timeConfig);

        repository.Update(timeConfig);  

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResult.Success(); 
    }

    public async Task<OperationResult> CreateTimeConfiguration(CreateTimeConfigurationRequest request, 
        CancellationToken cancellationToken)
    {
        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure(currentSemester.Error);

        if (!CheckConfigurationDateIsValid(request.TeamUpDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"TeamUpDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (!CheckConfigurationDateIsValid(request.TeamUpExpirationDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"TeamUpExpirationDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (!CheckConfigurationDateIsValid(request.RegistTopicDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"RegistTopicDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (!CheckConfigurationDateIsValid(request.RegistTopicExpiredDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"RegistTopicExpiredDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (string.IsNullOrEmpty(request.CampusId))
        {
            request.CampusId = currentUser.CampusId;
        }

        var timeConfig = new TimeConfiguration
        {
            RegistTopicDate = request.RegistTopicDate,
            RegistTopicExpiredDate = request.RegistTopicExpiredDate,
            TeamUpExpirationDate = request.TeamUpExpirationDate,
            TeamUpDate = request.TeamUpDate,    
            IsActived = request.IsActived,  
            CampusId = request.CampusId,    
        };

        repository.Insert(timeConfig);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    private static bool CheckConfigurationDateIsValid(DateTime configDate, Semester currentSemester)
    {
        return configDate >= currentSemester.StartDate && configDate <= currentSemester.EndDate;
    }
}
