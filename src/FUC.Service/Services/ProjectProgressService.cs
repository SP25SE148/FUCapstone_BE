using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.DTOs.ProjectProgressDTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class ProjectProgressService(
    ILogger<ProjectProgressService> logger,
    ICurrentUser currentUser,
    IUnitOfWork<FucDbContext> unitOfWork,
    IRepository<ProjectProgress> projectProgressRepository,
    IRepository<FucTask> fucTaskRepository,
    IRepository<Group> groupRepository,
    IRepository<WeeklyEvaluation> weeklyEvaluationRepository
    )
{
    private const int IndexStartProgressingRow = 2;

    public async Task<OperationResult> ImportProjectProgressFile(ImportProjectProgressRequest request, CancellationToken cancellationToken)
    {
        if (!IsValidFile(request.File))
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Invalid form file"));
        }

        var group = await groupRepository.GetAsync(
                x => x.Id == request.GroupId,
                include: x => x.Include(x => x.Capstone),
                null,
                cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        try
        {
            await unitOfWork.BeginTransactionAsync(cancellationToken);

            using var stream = new MemoryStream();

            await request.File.CopyToAsync(stream, cancellationToken);

            using XLWorkbook wb = new XLWorkbook(stream);

            // start reading the excel file
            IXLWorksheet workSheet = wb.Worksheet(1);

            var projectProgress = new ProjectProgress
            {
                GroupId = request.GroupId,
                SupervisorId = currentUser.UserCode,
                MeetingDate = workSheet.Cell(IndexStartProgressingRow, 1).GetValue<string>(),
            };

            int durationWeeks = group.Capstone.DurationWeeks;
            int startIndex = 2;

            // read the each projectProgressWeek of ProjectProgress
            for (int i = 1; i <= durationWeeks; i++)
            {
                var week = new ProjectProgressWeek
                {
                    TaskDescription = workSheet.Cell(IndexStartProgressingRow, ++startIndex).GetValue<string>(),
                    Status = ProjectProgressWeekStatus.InProgress,
                    WeekNumber = i,
                };

                projectProgress.ProjectProgressWeeks.Add(week);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to import the ProjectProgress with Error: {Message}", ex.Message);
            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create project progress fail."));
        }
    }

    public async Task<OperationResult> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        // TODO: Check the assignee and reporter are in the same group

        try
        {
            fucTaskRepository.Insert(new FucTask
            {
                KeyTask = request.KeyTask,
                Priority = request.Priority,
                Status = FucTaskStatus.ToDo,
                AssigneeId = request.AssigneeId,
                ReporterId = currentUser.UserCode,
                Description = request.Description,
                DueDate = request.DueDate,
                ProjectProgressWeekId = request.ProjectProgressWeekId
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Create Task with error: {Message}", ex.Message);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create task fail"));
        }
    }

    public async Task<OperationResult> CreateWeeklyEvaluation(CreateWeeklyEvaluationRequest request, CancellationToken cancellationToken)
    {
        // TODO: Check Supervisor is evaluation for 

        try
        {
            var evaluation = new WeeklyEvaluation
            {
                Comments = request.Comments,
                ContributionPercentage = request.ContributionPercentage,
                ProjectProgressWeekId= request.ProjectProgressWeekId,
                Status = request.Status,
                StudentId = request.StudentId, 
                SupervisorId = currentUser.UserCode
            };

            weeklyEvaluationRepository.Insert(evaluation);

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch(Exception ex)
        {
            logger.LogError("Create evaluation fail with error: {Message}", ex.Message);
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Evaluation weekly for this student fail."));
        }
    }

    public async Task<OperationResult<ProjectProgressDto>> GetProjectProgressByGroup(Guid groupId, CancellationToken cancellationToken)
    {
        var projectProgress = await projectProgressRepository.GetAsync(
            x => x.GroupId == groupId,
            include: x => x.Include(w => w.ProjectProgressWeeks),
            orderBy: null,
            cancellationToken
            );

        if (projectProgress == null)
        {
            return OperationResult.Failure<ProjectProgressDto>(new Error("ProjectProgress.Error", "Project Progress does not exist."));
        }

        return new ProjectProgressDto
        {
            Id = projectProgress.Id,
            MeetingDate = projectProgress.MeetingDate,
            ProjectProgressWeeks = projectProgress
                .ProjectProgressWeeks
                .OrderBy(p => p.WeekNumber)
                .Select(p => new ProjectProgressWeekDto
                {
                    Id = p.Id,
                    WeekNumber = p.WeekNumber,
                    MeetingContent = p.MeetingContent,
                    MeetingLocation = p.MeetingLocation,
                    Status = p.Status,
                    TaskDescription = p.TaskDescription,
                }).ToList(),
        };
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
            file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }
}
