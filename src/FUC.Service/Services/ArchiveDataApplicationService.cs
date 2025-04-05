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
    private readonly ILogger<ArchiveDataApplicationService> _logger;

    public ArchiveDataApplicationService(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<FucDbContext>();
        _integrationEventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        _currentUser = serviceProvider.GetRequiredService<ICurrentUser>();
        _semesterService = serviceProvider.GetRequiredService<ISemesterService>();
        _logger = serviceProvider.GetRequiredService<ILogger<ArchiveDataApplicationService>>();
    }

    public async Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(CancellationToken cancellationToken)
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

            await _context.Database.BeginTransactionAsync(cancellationToken);

            await _context.Groups
                .Where(x => x.CampusId == _currentUser.CampusId &&
                     x.CapstoneId == _currentUser.CapstoneId &&
                     x.SemesterId == currentSemester.Value.Id &&
                     x.Status == GroupStatus.Completed)
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

            return OperationResult.Failure<ExportCompletedStudents>(new Error("ArchiveData.Error", "Fail to archive data."));
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
                dataTable.Rows.Add(student);
            }

            using XLWorkbook wb = new XLWorkbook();

            wb.AddWorksheet(dataTable, "Students");

            using MemoryStream ms = new MemoryStream();

            wb.SaveAs(ms);
            ms.Position = 0;

            return ms.ToArray();
        }
        catch(Exception)
        {
            return OperationResult.Failure<byte[]>(new Error("ArchiveData.Error", "Fail progress to excel file."));
        }
    }
}
