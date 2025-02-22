using AutoMapper;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.SemesterDTO;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public sealed class SemesterService(ILogger<SemesterService> logger ,
    IMapper mapper,
    IUnitOfWork<FucDbContext> uow) : ISemesterService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Semester> _semesterRepository = uow.GetRepository<Semester>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    public async Task<OperationResult<string>> CreateSemesterAsync(CreateSemesterRequest request)
    {
        // check if the semester was existed
        Semester? semester = await _semesterRepository.GetAsync(
            s => s.Id.Equals(request.Id),
            cancellationToken:default);
        if (semester is not null) return OperationResult.Failure<string>(new Error("Error.Duplicate",$"The semester with id {semester.Id} was existed"));
        
        // create new semester
        Semester newSemester = new()
        {
            Id = request.Id.ToUpper(),
            Name = request.Name,
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };
        _semesterRepository.Insert(newSemester);
        await _uow.SaveChangesAsync();
        return newSemester.Id;
    }

    public async Task<OperationResult<SemesterResponse>> UpdateSemesterAsync(UpdateSemesterRequest request)
    {
        // Check if Semester exists
        Semester? semester = await _semesterRepository.GetAsync(
            s => s.Id.Equals(request.Id),
            cancellationToken: default);
        if (semester is null) return OperationResult.Failure<SemesterResponse>(Error.NullValue);

        // Update Semester fields
        semester.Name = request.Name;
        semester.StartDate = request.StartDate;
        semester.EndDate = request.EndDate;

        _semesterRepository.Update(semester);
        await _uow.SaveChangesAsync();
        return _mapper.Map<SemesterResponse>(semester);
    }

    public async Task<OperationResult<IEnumerable<SemesterResponse>>> GetSemestersAsync()
    {
        List<Semester> semesters = await _semesterRepository.GetAllAsync();
        return semesters.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<SemesterResponse>>(semesters))
            : OperationResult.Failure<IEnumerable<SemesterResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<SemesterResponse>>> GetAllActiveSemestersAsync()
    {
        IList<Semester> semesters = await _semesterRepository.FindAsync(s => !s.IsDeleted);
        return semesters.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<SemesterResponse>>(semesters))
            : OperationResult.Failure<IEnumerable<SemesterResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<SemesterResponse>> GetSemesterByIdAsync(string semesterId)
    {
        Semester? semester = await _semesterRepository.GetAsync(
            predicate: s => s.Id.Equals(semesterId),
            cancellationToken: default);
        return semester is not null
            ? OperationResult.Success(_mapper.Map<SemesterResponse>(semester))
            : OperationResult.Failure<SemesterResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteSemesterAsync(string semesterId)
    {
        Semester? semester = await _semesterRepository.GetAsync(
            predicate: s => s.Id.Equals(semesterId)
            , cancellationToken: default);
        
        if (semester is null) return OperationResult.Failure(Error.NullValue);

        semester.IsDeleted = true;
        semester.DeletedAt = DateTime.UtcNow;
        _semesterRepository.Update(semester);
        await _uow.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<Semester>> GetCurrentSemesterAsync()
    {
        var currentSemester = await _semesterRepository.GetAsync(x => DateTime.UtcNow >= x.StartDate 
            && DateTime.UtcNow <= x.EndDate, 
            cancellationToken: default);

        if (currentSemester is null)
        {
            logger.LogInformation("The current semester is not going on.");
            return OperationResult.Failure<Semester>(new Error("Semester.Error", "No semester is going on."));
        }

        return OperationResult.Success(currentSemester);
    }
}
