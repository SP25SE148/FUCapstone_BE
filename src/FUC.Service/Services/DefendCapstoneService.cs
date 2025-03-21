using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class DefendCapstoneService(
    ILogger<DefendCapstoneService> logger,
    ICurrentUser currentUser,
    IRepository<DefendCapstoneProjectInformationCalendar> defendCapstoneCalendarRepository,
    IRepository<DefendCapstoneProjectCouncilMember> defendCapstoneCouncilMemberRepository,
    IUnitOfWork<FucDbContext> unitOfWork,
    IIntegrationEventLogService integrationEventLogService,
    S3BucketConfiguration s3BucketConfiguration,
    ICapstoneService capstoneService,
    ITopicService topicService,
    IS3Service s3Service)
{
    private const int MaxAttemptTimesToDefendCapstone = 2;

    public async Task<OperationResult> UploadDenfendCapstoneProjectCalendar(IFormFile file, CancellationToken cancellationToken)
    {
        if (!IsValidFile(file))
        {
            return OperationResult.Failure(new Error("DenfendCapstoneProjectCalendar.Error", "Invalid form file."));
        }

        var capstone = await capstoneService.GetCapstoneByIdAsync(currentUser.CapstoneId);
        if (capstone.IsFailure)
            return OperationResult.Failure(new Error("Error.SemesterIsNotGoingOn",
                "The current semester is not going on"));

        try
        {
            var defendCalendars = await ParseDefendCapstoneCalendarsFromFile(file, capstone.Value.Id, cancellationToken);
            defendCapstoneCalendarRepository.InsertRange(defendCalendars);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("import review failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.ImportFailed", "import review failed"));
        }
    }

    private async Task<List<DefendCapstoneProjectInformationCalendar>> ParseDefendCapstoneCalendarsFromFile(IFormFile file, string capstoneId, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        using var wb = new XLWorkbook(stream);
        IXLWorksheet workSheet = wb.Worksheet(1);

        var defendCalendars = new List<DefendCapstoneProjectInformationCalendar>();

        foreach (var row in workSheet.Rows().Skip(2))
        {
            var topicCode = row.Cell(2).GetValue<string>();

            if (string.IsNullOrEmpty(topicCode))
            {
                break;
            }

            var topicResult = await topicService.GetTopicByCode(topicCode, cancellationToken);

            if (topicResult.IsFailure || 
                topicResult.Value.Status != TopicStatus.Approved || 
                !topicResult.Value.IsAssignedToGroup || 
                topicResult.Value.CapstoneId != capstoneId)
                throw new InvalidOperationException("Topic is not available for defend pharse.");
        }

        return defendCalendars;
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }

    public async Task<OperationResult<object>> GetDefendCalendersByCouncilMember(CancellationToken cancellationToken)
    {
        var defendCalendarsOfMember = await defendCapstoneCouncilMemberRepository.FindAsync(
            x => x.SupervisorId == currentUser.UserCode,
            include: x => x.Include(x => x.DefendCapstoneProjectInformationCalendar),
            orderBy: x => x.OrderBy(x => x.DefendCapstoneProjectInformationCalendar.Slot), 
            cancellationToken);

        ArgumentNullException.ThrowIfNull(defendCalendarsOfMember);

        return null;
    }
}

public class DefendCapstoneCalendarResponse
{
    public Guid Id { get; set; }
    public Guid TopicId { get; set; }
    public Guid GroupId { get; set; }
    public string GroupCode { get; set; }
    public required string TopicCode { get; set; }
    public string CampusId { get; set; }
    public string SemesterId { get; set; }
    public int DefendAttempt { get; set; }
    public string Location { get; set; } // Room
    public int Slot { get; set; }
    public DateTime DefenseDate { get; set; }
}
