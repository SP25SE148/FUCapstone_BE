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
        var currentSemester = await semesterService.GetCurrentSemesterAsync();  

        if (currentSemester.IsFailure) 
            return OperationResult.Failure<IList<TimeConfigurationDto>>(currentSemester.Error);

        var result = await repository.FindAsync(
            x => (currentUser.CapstoneId == "all" || x.CapstoneId == currentUser.CapstoneId) &&
                x.SemesterId == currentSemester.Value.Id,
            include: null,
            orderBy: null,
            selector: x => new TimeConfigurationDto
            {
                Id = x.Id,
                CapstoneId = x.CapstoneId,  
                SemesterId = x.SemesterId,
                RegistTopicDate = x.RegistTopicDate,    
                RegistTopicExpiredDate = x.RegistTopicExpiredDate,
                TimeUpDate = x.TimeUpDate,  
                TimeUpExpirationDate = x.TimeUpExpirationDate,  
            });

        return OperationResult.Success(result);
    }

    public async Task<OperationResult<TimeConfigurationDto>> GetCurrentTimeConfiguration(string capstoneId)
    {
        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<TimeConfigurationDto>(currentSemester.Error);

        var result = await repository.GetAsync(
            x => x.CapstoneId == capstoneId &&
                x.SemesterId == currentSemester.Value.Id,
            selector: x => new TimeConfigurationDto
            {
                Id = x.Id,
                CapstoneId = x.CapstoneId,
                SemesterId = x.SemesterId,
                RegistTopicDate = x.RegistTopicDate,
                RegistTopicExpiredDate = x.RegistTopicExpiredDate,
                TimeUpDate = x.TimeUpDate,
                TimeUpExpirationDate = x.TimeUpExpirationDate,
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

        if (timeConfig.SemesterId != currentSemester.Value.Id)
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                "Only update the timeconfig of CurrentSemester"));

        if (request.TimeUpDate.HasValue && 
            CheckConfigurationDateIsValid(request.TimeUpDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"TimeUpDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.TimeUpExpirationDate.HasValue && 
            CheckConfigurationDateIsValid(request.TimeUpExpirationDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"TimeUpExpirationDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.RegistTopicDate.HasValue && 
            CheckConfigurationDateIsValid(request.RegistTopicDate.Value, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error",
                $"RegistTopicDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (request.RegistTopicExpiredDate.HasValue && 
            CheckConfigurationDateIsValid(request.RegistTopicExpiredDate.Value, currentSemester.Value))
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

        if (CheckConfigurationDateIsValid(request.TimeUpDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"TimeUpDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (CheckConfigurationDateIsValid(request.TimeUpExpirationDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"TimeUpExpirationDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (CheckConfigurationDateIsValid(request.RegistTopicDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"RegistTopicDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (CheckConfigurationDateIsValid(request.RegistTopicExpiredDate, currentSemester.Value))
            return OperationResult.Failure(new Error("TimeConfiguration.Error", 
                $"RegistTopicExpiredDate need to in the duration of Semester {currentSemester.Value.Id}"));

        if (string.IsNullOrEmpty(request.CapstoneId))
        {
            request.CapstoneId = currentUser.CapstoneId;
        }

        var timeConfig = new TimeConfiguration
        {
            RegistTopicDate = request.RegistTopicDate,
            RegistTopicExpiredDate = request.RegistTopicExpiredDate,    
            TimeUpExpirationDate = request.TimeUpExpirationDate,    
            TimeUpDate = request.TimeUpDate,    
            IsActived = request.IsActived,  
            CapstoneId = request.CapstoneId,    
            SemesterId = currentSemester.Value.Id,
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
