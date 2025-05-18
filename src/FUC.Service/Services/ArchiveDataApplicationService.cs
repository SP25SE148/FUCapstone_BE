using System.Data;
using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.ConfigDTO;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class ArchiveDataApplicationService : IArchiveDataApplicationService
{
    private readonly FucDbContext _context;
    private readonly IIntegrationEventLogService _integrationEventLogService;
    private readonly ICurrentUser _currentUser;
    private readonly ISemesterService _semesterService;
    private readonly ISystemConfigurationService _systemConfigurationService;
    private readonly ILogger<ArchiveDataApplicationService> _logger;

    public ArchiveDataApplicationService(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<FucDbContext>();
        _integrationEventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        _currentUser = serviceProvider.GetRequiredService<ICurrentUser>();
        _semesterService = serviceProvider.GetRequiredService<ISemesterService>();
        _logger = serviceProvider.GetRequiredService<ILogger<ArchiveDataApplicationService>>();
        _systemConfigurationService = serviceProvider.GetRequiredService<ISystemConfigurationService>();
    }

    public async Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(
        CancellationToken cancellationToken)
    {
        try
        {
            var currentSemester = await _semesterService.GetCurrentSemesterAsync();

            if (currentSemester.IsFailure)
                return OperationResult.Failure<ExportCompletedStudents>(currentSemester.Error);

            var students = await _context.Students
                .Where(x => x.CampusId == _currentUser.CampusId &&
                            x.CapstoneId == _currentUser.CapstoneId)
                .ToListAsync(cancellationToken);

            if (students.Count == 0)
                return OperationResult.Failure<ExportCompletedStudents>(Error.NullValue);

            if (await _context.Groups.AnyAsync(x => x.Status == GroupStatus.InProgress ||
                                                    x.Status == GroupStatus.Pending, cancellationToken))
                return OperationResult.Failure<ExportCompletedStudents>(new Error("ArchiveData.Error",
                    "They have some Inprogress groups, so that can not archive data."));

            await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.Groups
                .Where(x => x.CampusId == _currentUser.CampusId &&
                            x.CapstoneId == _currentUser.CapstoneId &&
                            x.SemesterId == currentSemester.Value.Id)
                .ExecuteDeleteAsync(cancellationToken);

            await _context.Students
                .Where(x => x.CampusId == _currentUser.CampusId &&
                            x.CapstoneId == _currentUser.CapstoneId)
                .ExecuteDeleteAsync(cancellationToken);

            var resultFile = ArchiveStudentsInExcel(students);

            if (resultFile.IsFailure)
                throw new InvalidOperationException("Export to file fail.");

            _integrationEventLogService.SendEvent(new ArchiveDataStudentsEvent
            {
                StudentsCode = students.Select(x => x.Id)
            });

            await _context.SaveChangesAsync(cancellationToken);

            await _context.Database.CommitTransactionAsync(cancellationToken);

            return OperationResult.Success(new ExportCompletedStudents
            {
                Content = resultFile.Value,
                FileName = $"Students_{_currentUser.CampusId}_{currentSemester}_{_currentUser.CapstoneId}.xlsx"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("Fail to archive data with error: {Message}", ex.Message);

            await _context.Database.RollbackTransactionAsync(cancellationToken);

            return OperationResult.Failure<ExportCompletedStudents>(new Error("ArchiveData.Error",
                "Fail to archive data."));
        }
    }

    private static OperationResult<byte[]> ArchiveStudentsInExcel(List<Student> students)
    {
        try
        {
            using var dataTable = new DataTable
            {
                TableName = nameof(Student)
            };

            dataTable.Columns.Add("Id", typeof(string));
            dataTable.Columns.Add("FullName", typeof(string));
            dataTable.Columns.Add("Email", typeof(string));
            dataTable.Columns.Add("Status", typeof(string));
            dataTable.Columns.Add("CampusId", typeof(string));
            dataTable.Columns.Add("MajorId", typeof(string));
            dataTable.Columns.Add("CapstoneId", typeof(string));

            foreach (Student student in students)
            {
                dataTable.Rows.Add(student.Id,
                    student.FullName,
                    student.Email,
                    student.Status.ToString(),
                    student.CampusId,
                    student.MajorId,
                    student.CapstoneId);
            }

            using XLWorkbook wb = new XLWorkbook();

            wb.AddWorksheet(dataTable, "Students");

            using MemoryStream ms = new MemoryStream();

            wb.SaveAs(ms);
            ms.Position = 0;

            return ms.ToArray();
        }
        catch (Exception)
        {
            return OperationResult.Failure<byte[]>(new Error("ArchiveData.Error", "Fail progress to excel file."));
        }
    }

    public async Task<OperationResult<SuperAdminDashBoardDto>> PresentSuperAdminDashBoard(
        string semesterId, CancellationToken cancellationToken)
    {
        var currentSemester = await _semesterService.GetSemesterByIdAsync(semesterId);

        if (currentSemester.IsFailure)
            return OperationResult.Failure<SuperAdminDashBoardDto>(new Error("GetDashBoard.Error",
                "Semester not found."));

        var students = await _context.Students.ToListAsync(cancellationToken);
        var supervisors = await _context.Supervisors.ToListAsync(cancellationToken);
        var numberOfCapstones = await _context.Capstones.CountAsync(cancellationToken);

        var topics = await _context.Topics
            .Where(x => x.SemesterId == currentSemester.Value.Id)
            .ToListAsync(cancellationToken);

        var groups = await _context.Groups
            .Where(x => x.SemesterId == currentSemester.Value.Id)
            .ToListAsync(cancellationToken);

        var campusIds = await _context
            .Campuses
            .OrderBy(x => x.Id)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var data = new Dictionary<string, DashBoardDto>();

        foreach (var id in campusIds)
        {
            data[id] = new DashBoardDto
            {
                Groups = groups.Count(x => x.CampusId == id),
                Topics = topics.Count(x => x.CampusId == id),
                Students = students.Count(x => x.CampusId == id),
                Supervisors = supervisors.Count(x => x.CampusId == id),
            };
        }

        return new SuperAdminDashBoardDto
        {
            Students = students.Count,
            Supervisors = supervisors.Count,
            Groups = groups.Count,
            Topics = topics.Count,
            Capstones = numberOfCapstones,
            DataPerCampus = data,
        };
    }

    public async Task<OperationResult<AdminDashBoardDto>> PresentAdminDashBoard(string semesterId,
        CancellationToken cancellationToken)
    {
        var currentSemester = await _semesterService.GetSemesterByIdAsync(semesterId);

        if (currentSemester.IsFailure)
            return OperationResult.Failure<AdminDashBoardDto>(new Error("GetDashBoard.Error", "Semester not found."));

        var students = await _context.Students
            .Where(x => x.CampusId == _currentUser.CampusId)
            .CountAsync(cancellationToken);


        var supervisors = await _context.Supervisors
            .Where(x => x.CampusId == _currentUser.CampusId)
            .ToListAsync(cancellationToken);

        var topics = await _context.Topics
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.SemesterId == currentSemester.Value.Id)
            .ToListAsync(cancellationToken);

        var groups = await _context.Groups
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.SemesterId == currentSemester.Value.Id)
            .CountAsync(cancellationToken);

        return new AdminDashBoardDto
        {
            Students = students,
            Supervisors = supervisors.Count,
            Topics = topics.Count,
            Groups = groups,
            SupervisorsInEachMajor = supervisors
                .GroupBy(x => x.MajorId)
                .ToDictionary(g => g.Key, g => g.Count()),
            TopicsInEachCapstone = topics
                .GroupBy(x => x.CapstoneId)
                .ToDictionary(g => g.Key, g => g.Count()),
            MaxTopicsOfCapstoneEachMajor = await _systemConfigurationService.GetMinimumTopicsByMajorId(),
        };
    }

    public async Task<OperationResult<ManagerDashBoardDto>> PresentManagerDashBoard(string semesterId,
        CancellationToken cancellationToken)
    {
        var currentSemester = await _semesterService.GetSemesterByIdAsync(semesterId);

        if (currentSemester.IsFailure)
            return OperationResult.Failure<ManagerDashBoardDto>(new Error("GetDashBoard.Error", "Semester not found."));


        var students = await _context.Students
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.CapstoneId == _currentUser.CapstoneId)
            .CountAsync(cancellationToken);

        var supervisors = await _context.Supervisors
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.MajorId == _currentUser.MajorId)
            .ToListAsync(cancellationToken);

        var topics = await _context.Topics
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.SemesterId == currentSemester.Value.Id &&
                        x.CapstoneId == _currentUser.CapstoneId)
            .ToListAsync(cancellationToken);

        var topicsPerSupervisor = topics
            .GroupBy(t => t.MainSupervisorId)
            .ToDictionary(g => g.Key, g => g.Count());

        var groups = await _context.Groups
            .Where(x => x.CampusId == _currentUser.CampusId &&
                        x.SemesterId == currentSemester.Value.Id &&
                        x.CapstoneId == _currentUser.CapstoneId)
            .AsSplitQuery()
            .Include(group => group.ProjectProgress)
            .ThenInclude(projectProgress => projectProgress.FucTasks)
            .Include(group => group.GroupMembers)
            .ToListAsync(cancellationToken: cancellationToken);

        var totalTasks = groups.Sum(g => g.ProjectProgress?.FucTasks?.Count ?? 0);

        var completedTasks = groups.Sum(g =>
            g.ProjectProgress?.FucTasks?.Count(t => t.Status == FucTaskStatus.Done) ?? 0);

        var overdueTasks = groups.Sum(g =>
            g.ProjectProgress?.FucTasks?
                .Count(t => t.DueDate < DateTime.Now && t.Status != FucTaskStatus.Done) ?? 0);

        var averageGroupSize = groups.Count > 0
            ? groups.Where(x => x.GroupMembers != null && x.GroupMembers.Count > 0)
                .Average(g => g.GroupMembers.Count)
            : 0;

        var taskCompletionRate = totalTasks > 0 ? (double)completedTasks / totalTasks : 0;

        var bestPerformingGroup = groups
            .Select(g => new GroupTaskMetrics
            {
                GroupId = g.Id,
                GroupCode = g.GroupCode,
                TotalTasks = g.ProjectProgress?.FucTasks?.Count ?? 0,
                CompletedTasks = g.ProjectProgress?.FucTasks?
                    .Count(t => t.Status == FucTaskStatus.Done) ?? 0,
                OverdueTasks =
                    g.ProjectProgress?.FucTasks?
                        .Count(t => t.DueDate < DateTime.Now && t.Status != FucTaskStatus.Done) ?? 0
            })
            .OrderByDescending(gm => gm.CompletedTasks)
            .FirstOrDefault();

        var worstPerformingGroup = groups
            .Select(g => new GroupTaskMetrics
            {
                GroupId = g.Id,
                GroupCode = g.GroupCode,
                TotalTasks = g.ProjectProgress?.FucTasks?.Count ?? 0,
                CompletedTasks = g.ProjectProgress?.FucTasks?
                    .Count(t => t.Status == FucTaskStatus.Done) ?? 0,
                OverdueTasks =
                    g.ProjectProgress?.FucTasks?
                        .Count(t => t.DueDate < DateTime.Now && t.Status != FucTaskStatus.Done) ?? 0
            })
            .OrderBy(gm => gm.CompletedTasks)
            .FirstOrDefault();

        return new ManagerDashBoardDto
        {
            Students = students,
            Supervisors = supervisors.Count,
            Topics = topics.Count,
            Groups = groups.Count,
            TopicsInEachStatus = topics
                .GroupBy(x => x.Status.ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            AverageGroupSize = averageGroupSize,
            TaskCompletionRate = taskCompletionRate,
            OverdueTaskCount = overdueTasks,
            BestPerformingGroup = bestPerformingGroup,
            WorstPerformingGroup = worstPerformingGroup,
            TopicsPerSupervisor = topicsPerSupervisor,
            MaxTopicsOfCapstone = _systemConfigurationService.GetGetMaxTopicsOfCapstone(_currentUser.CapstoneId),
        };
    }
}
