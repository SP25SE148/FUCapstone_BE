using FUC.Common.Abstractions;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data.Data;
using FUC.Data.Enums;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DocumentDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FUC.Service.Services;

public class ArchiveDataApplicationService
{
    private readonly FucDbContext _context;
    private readonly IIntegrationEventLogService _integrationEventLogService;
    private readonly ICurrentUser _currentUser;
    private readonly ISemesterService _semesterService;

    public ArchiveDataApplicationService(IServiceProvider serviceProvider)
    {
        _context = serviceProvider.GetRequiredService<FucDbContext>();
        _integrationEventLogService = serviceProvider.GetRequiredService<IIntegrationEventLogService>();
        _currentUser = serviceProvider.GetRequiredService<ICurrentUser>();
        _semesterService = serviceProvider.GetRequiredService<ISemesterService>();
    }

    public async Task<OperationResult<ExportCompletedStudents>> ArchiveDataCompletedStudents(CancellationToken cancellationToken)
    {
        var currentSemester = await _semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<ExportCompletedStudents>(currentSemester.Error);

        var groups = await _context.Groups
            .Where(x => x.CampusId == _currentUser.CampusId &&
                 x.CapstoneId == _currentUser.CapstoneId &&
                 x.SemesterId == currentSemester.Value.Id &&
                 x.Status == GroupStatus.Completed &&
                 !x.IsDeleted)
            .ToListAsync(cancellationToken);

        if (groups.Count == 0)
            return OperationResult.Failure<ExportCompletedStudents>(Error.NullValue);

        return OperationResult.Success(new ExportCompletedStudents { });
    }
}
