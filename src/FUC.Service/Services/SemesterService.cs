﻿using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
using FUC.Common.Helpers;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.SemesterDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace FUC.Service.Services;

public sealed class SemesterService(
    ILogger<SemesterService> logger,
    IMapper mapper,
    IRepository<Semester> semesterRepository,
    IUnitOfWork<FucDbContext> uow,
    ITimeConfigurationService timeConfigurationService) : ISemesterService
{
    public async Task<OperationResult<string>> CreateSemesterAsync(CreateSemesterRequest request)
    {
        // check if the semester was existed
        var semester = await semesterRepository.GetAsync(
            s => s.Id == request.Id,
            cancellationToken: default);

        if (semester is not null)
            return OperationResult.Failure<string>(new Error("Error.Duplicate",
                $"The semester with id {semester.Id} was existed"));

        // create new semester
        Semester newSemester = new()
        {
            Id = request.Id.ToUpper(),
            Name = request.Name,
            StartDate = request.StartDate.StartOfDay(),
            EndDate = request.EndDate.EndOfDay(),
        };

        semesterRepository.Insert(newSemester);

        await uow.SaveChangesAsync();
        return newSemester.Id;
    }

    public async Task<OperationResult<SemesterResponse>> UpdateSemesterAsync(UpdateSemesterRequest request)
    {
        // Check if Semester exists
        var semester = await semesterRepository.GetAsync(
            s => s.Id == request.Id,
            cancellationToken: default);

        if (semester is null)
            return OperationResult.Failure<SemesterResponse>(Error.NullValue);

        // Update Semester fields
        semester.Name = request.Name;
        semester.StartDate = request.StartDate.StartOfDay();
        semester.EndDate = request.EndDate.EndOfDay();

        semesterRepository.Update(semester);
        await uow.SaveChangesAsync();

        return mapper.Map<SemesterResponse>(semester);
    }

    public async Task<OperationResult<IEnumerable<SemesterResponse>>> GetSemestersAsync()
    {
        List<Semester> semesters = (await semesterRepository.GetAllAsync())
            .OrderByDescending(x => x.StartDate)
            .ToList();

        return OperationResult.Success(mapper.Map<IEnumerable<SemesterResponse>>(semesters));
    }

    public async Task<OperationResult<IEnumerable<SemesterResponse>>> GetAllActiveSemestersAsync()
    {
        IList<Semester> semesters = await semesterRepository.FindAsync(s => !s.IsDeleted);
        return OperationResult.Success(mapper.Map<IEnumerable<SemesterResponse>>(semesters));
    }

    public async Task<OperationResult<SemesterResponse>> GetSemesterByIdAsync(string semesterId)
    {
        Semester? semester = await semesterRepository.GetAsync(
            predicate: s => s.Id == semesterId,
            cancellationToken: default);

        return semester is not null
            ? OperationResult.Success(mapper.Map<SemesterResponse>(semester))
            : OperationResult.Failure<SemesterResponse>(Error.NullValue);
    }

    public async Task<OperationResult> DeleteSemesterAsync(string semesterId)
    {
        Semester? semester = await semesterRepository.GetAsync(
            predicate: s => s.Id == semesterId,
            cancellationToken: default);

        if (semester is null)
            return OperationResult.Failure(Error.NullValue);

        semester.IsDeleted = true;
        semester.DeletedAt = DateTime.Now;
        semesterRepository.Update(semester);

        await uow.SaveChangesAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<Semester>> GetCurrentSemesterAsync(bool isEnableTracking = false)
    {
        var currentSemester = await semesterRepository
            .GetAsync(x => DateTime.Now >= x.StartDate &&
                           DateTime.Now <= x.EndDate,
                isEnableTracking,
                include: x => x.Include(x => x.TimeConfigurations));

        if (currentSemester is null)
        {
            logger.LogInformation("The current semester is not going on.");
            return OperationResult.Failure<Semester>(new Error("Semester.Error", "No semester is going on."));
        }

        return OperationResult.Success(currentSemester);
    }

    public async Task<OperationResult<Semester?>> GetNextSemesterAsync(bool isEnableTracking = false)
    {
        var semester = (await semesterRepository.FindAsync(
            t => DateTime.Now < t.StartDate,
            null,
            isEnableTracking)).MinBy(x => x.StartDate);
        if (semester is null)
            return OperationResult.Success<Semester?>(null);
        var timeConfig = await timeConfigurationService.GetTimeConfigurationBySemesterId(semester.Id);
        if (timeConfig.IsSuccess)
            return OperationResult.Success<Semester?>(null);
        return semester;
    }

    public async Task<OperationResult<IEnumerable<SemesterResponse>>> GetSemestersBetweenCurrentDate()
    {
        var resultList = new List<Semester?>();
        var previousSemester = (await semesterRepository.FindAsync(s => s.StartDate <= DateTime.Now,
            orderBy: s => s.OrderByDescending(s => s.EndDate))).FirstOrDefault();
        var nextSemester = (await semesterRepository.FindAsync(
            t => DateTime.Now < t.StartDate)).MinBy(x => x.StartDate);
        if (previousSemester != null)
        {
            resultList.Add(previousSemester);
        }

        if (nextSemester != null)
        {
            resultList.Add(nextSemester);
        }

        return OperationResult.Success(mapper.Map<IEnumerable<SemesterResponse>>(resultList));
    }

    public async Task<OperationResult<Semester?>> GetSemesterForCreateTopic()
    {
        var semester = await GetCurrentSemesterAsync();
        return semester.IsSuccess
            ? semester
            : (await semesterRepository.FindAsync(
                t => DateTime.Now < t.StartDate,
                include: x => x.Include(x => x.TimeConfigurations))).MinBy(x => x.StartDate);
    }

    public async Task<List<string>> GetPreviouseSemesterIds(
        DateTime? startDayOfCurrentSemester = null, int top = 3)
    {
        startDayOfCurrentSemester ??= DateTime.Now;

        var previousSemesterIds = await semesterRepository.FindAsync(
            predicate: s => s.StartDate < startDayOfCurrentSemester,
            null,
            orderBy: s => s.OrderByDescending(s => s.StartDate),
            selector: x => x.Id,
            top: top,
            default);

        return [.. previousSemesterIds];
    }
}
