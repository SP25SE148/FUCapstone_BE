using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DefendCapstone;
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
    ICapstoneService capstoneService,
    ITopicService topicService,
    ISupervisorService supervisorService,
    IDocumentsService documentsService) : IDefendCapstoneService
{
    private const int MaxAttemptTimesToDefendCapstone = 2;

    public async Task<OperationResult> UploadDefendCapstoneProjectCalendar(IFormFile file,
        CancellationToken cancellationToken)
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
            var defendCalendars =
                await ParseDefendCapstoneCalendarsFromFile(file, capstone.Value.Id, cancellationToken);
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

    private async Task<List<DefendCapstoneProjectInformationCalendar>> ParseDefendCapstoneCalendarsFromFile(
        IFormFile file, string capstoneId, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        using var wb = new XLWorkbook(stream);
        IXLWorksheet workSheet = wb.Worksheet(1);

        var defendCalendars = new List<DefendCapstoneProjectInformationCalendar>();
        var memberColumn = 8;
        foreach (var row in workSheet.Rows().Skip(2))
        {
            var topicCode = row.Cell(2).GetValue<string>();
            var memberInfoList = new List<string>();
            if (string.IsNullOrEmpty(topicCode))
            {
                break;
            }

            var topicResult = await topicService.GetTopicByCode(topicCode, cancellationToken);
            if (topicResult.IsFailure ||
                topicResult.Value.Status != TopicStatus.Approved ||
                !topicResult.Value.IsAssignedToGroup ||
                topicResult.Value.CapstoneId != capstoneId)
                throw new InvalidOperationException("Topic is not available for defend phase.");

            var presidentInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(6).GetValue<string>());
            if (presidentInformation.IsFailure)
                throw new InvalidOperationException("President is not available for defend phase.");

            var secretaryInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(7).GetValue<string>());
            if (secretaryInformation.IsFailure)
                throw new InvalidOperationException("Secretary is not available for defend phase.");

            if (topicResult.Value.MainSupervisorId.Equals(presidentInformation.Value.Id) ||
                topicResult.Value.MainSupervisorId.Equals(secretaryInformation.Value.Id))
                throw new Exception("Can not assign this supervisor for their own topic.");


            for (; memberColumn < row.Cells().Count(); memberColumn++)
            {
                if (!string.IsNullOrEmpty(workSheet.Cell(2, memberColumn).GetValue<string>()))
                {
                    var memberInfo =
                        await supervisorService.GetSupervisorByIdAsync(row.Cell(memberColumn).GetValue<string>());
                    if (memberInfo.IsFailure)
                        throw new Exception("member information is invalid ");
                    if (topicResult.Value.MainSupervisorId.Equals(memberInfo.Value.Id))
                        throw new Exception("Can not assign this supervisor for their own topic.");

                    memberInfoList
                        .Add(memberInfo.Value.Id);
                    // check if member information is duplicate value with president or secretary
                    if (memberInfoList.Any(
                            x => x == secretaryInformation.Value.Id || x == presidentInformation.Value.Id))
                        throw new Exception("Member information is duplicate value with president or secretary.");
                }
            }
        }

        return defendCalendars;
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
               file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase));
    }

    public async Task<OperationResult<List<(DateTime, List<DefendCapstoneCalendarResponse>)>>>
        GetDefendCalendersByCouncilMember(CancellationToken cancellationToken)
    {
        var result = new List<(DateTime, List<DefendCapstoneCalendarResponse>)>();

        var defendCalendarsIdsOfMember = await defendCapstoneCouncilMemberRepository.FindAsync(
            x => x.SupervisorId == currentUser.UserCode,
            include: null,
            orderBy: null,
            selector: x => x.DefendCapstoneProjectInformationCalendarId,
            cancellationToken);

        if (defendCalendarsIdsOfMember is null || defendCalendarsIdsOfMember.Count == 0)
            return result;

        var calendars = await defendCapstoneCalendarRepository.FindAsync(
            x => defendCalendarsIdsOfMember.Contains(x.Id),
            include: x => x.Include(x => x.DefendCapstoneProjectMemberCouncils)
                                .ThenInclude(x => x.Supervisor)
                            .Include(x => x.Topic),
            cancellationToken);

        if (calendars is null || calendars.Count == 0)
            return result;

        foreach (var calendar in calendars.GroupBy(x => x.DefenseDate))
        {
            result.Add((calendar.Key, calendar.Select(x => new DefendCapstoneCalendarResponse
            {
                Id = x.Id,
                TopicId = x.TopicId,
                DefenseDate = x.DefenseDate,
                DefendAttempt = x.DefendAttempt,
                Location = x.Location,
                Slot = x.Slot,
                CampusId = x.CampusId,
                SemesterId = x.SemesterId,
                TopicCode = x.TopicCode,
                CouncilMembers = x.DefendCapstoneProjectMemberCouncils.Select(x => new DefendCapstoneCouncilMemberDto
                {
                    Id = x.Id,
                    IsPresident = x.IsPresident,
                    IsSecretary = x.IsSecretary,
                    SupervisorId = x.SupervisorId,
                    SupervisorName = x.Supervisor.FullName,
                    DefendCapstoneProjectInformationCalendarId = x.DefendCapstoneProjectInformationCalendarId
                }).ToList(),
            })
            .OrderBy(x => x.Slot)
            .ToList()));
        }

        return result.OrderBy(x => x.Item1).ToList();
    }

    public async Task<OperationResult> UploadThesisCouncilMeetingMinutesForDefendCapstone(
        UploadThesisCouncilMeetingMinutesRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!IsValidFile(request.File))
            {
                return OperationResult.Failure(new Error("DenfendCapstoneProjectCalendar.Error", "Invalid form file."));
            }

            var calendar = await defendCapstoneCalendarRepository.GetAsync(
                x => x.Id == request.DefendCapstoneCalendarId,
                include: x => x.Include(x => x.DefendCapstoneProjectMemberCouncils)
                    .Include(x => x.Topic),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(calendar);

            if (calendar.DefendCapstoneProjectMemberCouncils
                    .Any(x => x.SupervisorId == currentUser.UserCode && (x.IsPresident || x.IsSecretary)))
                return OperationResult.Failure(new Error("DefendCapstone.Error",
                    "Only who has the permission can do this action."));

            var key = $"{calendar.CampusId}/{calendar.SemesterId}/{calendar.Topic.CapstoneId}/{calendar.TopicCode}/{calendar.DefendAttempt}";

            calendar.IsUploadedThesisMinute = true;

            defendCapstoneCalendarRepository.Update(calendar);

            if ((await documentsService.CreateThesisDocument(request.File, key, cancellationToken)).IsFailure)
                throw new InvalidOperationException("Upload thesis fail.");
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex) 
        {
            logger.LogError("Can not upload the thesis Id {Thesis} with error: {Message}", request.DefendCapstoneCalendarId, ex.Message);
            return OperationResult.Failure(new Error("DefendCapstone.Error", "Can not upload thesis document."));
        }
    }

    public async Task<OperationResult<string>> PresentThesisForTopicResignedUrl(Guid calendarId, CancellationToken cancellationToken)
    {
        var calendar = await defendCapstoneCalendarRepository.GetAsync(
            x => x.Id == calendarId,
            include: x => x.Include(x => x.DefendCapstoneProjectMemberCouncils)
                .Include(x => x.Topic),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(calendar);

        if (currentUser.Role == UserRoles.Student)
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You do not have the permission."));

        if (currentUser.Role == UserRoles.Supervisor && 
            calendar.DefendCapstoneProjectMemberCouncils.Any(x => x.SupervisorId == currentUser.UserCode))
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You can not get this thesis because you are not in the Council."));

        var key = $"{calendar.CampusId}/{calendar.SemesterId}/{calendar.Topic.CapstoneId}/{calendar.TopicCode}/{calendar.DefendAttempt}";

        return await documentsService.PresentThesisCouncilMeetingMinutesForTopicPresignedUrl(key);
    }

    public async Task<OperationResult> UpdateStatusOfGroupAfterDefend(Guid calendarId, CancellationToken cancellationToken)
    {
        var calendar = await defendCapstoneCalendarRepository.GetAsync(
            x => x.Id == calendarId,
            include: x => x.Include(x => x.DefendCapstoneProjectMemberCouncils)
                .Include(x => x.Topic),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(calendar);

        if (!calendar.IsUploadedThesisMinute)
            return OperationResult.Failure(new Error("DefendCapstone.Error","The thesis file of that was not uploaded."));

        if (calendar.DefendCapstoneProjectMemberCouncils
            .Any(x => x.SupervisorId == currentUser.UserCode && x.IsPresident))
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You can not get this thesis because you are not in the Council."));

        //TODO: Update status of group

        return OperationResult.Success();
    }
}
