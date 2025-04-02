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
using FUC.Service.DTOs.SupervisorDTO;
using FUC.Service.DTOs.DefendCapstone;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FUC.Service.DTOs.GroupDTO;

namespace FUC.Service.Services;

public class DefendCapstoneService(
    ILogger<DefendCapstoneService> logger,
    ICurrentUser currentUser,
    IRepository<DefendCapstoneProjectInformationCalendar> defendCapstoneCalendarRepository,
    IRepository<DefendCapstoneProjectCouncilMember> defendCapstoneCouncilMemberRepository,
    IUnitOfWork<FucDbContext> unitOfWork,
    IIntegrationEventLogService integrationEventLogService,
    ISemesterService semesterService,
    ITopicService topicService,
    IGroupService groupService,
    ISupervisorService supervisorService,
    ISystemConfigurationService systemConfigService,
    IDocumentsService documentsService) : IDefendCapstoneService
{
    public async Task<OperationResult> UploadDefendCapstoneProjectCalendar(IFormFile file,
        CancellationToken cancellationToken)
    {
        if (!IsValidFile(file))
        {
            return OperationResult.Failure(new Error("DenfendCapstoneProjectCalendar.Error", "Invalid form file."));
        }

        try
        {
            var currentSemester = await semesterService.GetCurrentSemesterAsync();
            if (currentSemester.IsFailure)
                return OperationResult.Failure(new Error("ImportFailed", "Current semester is not on going!!"));
            var defendCalendars =
                await ParseDefendCapstoneCalendarsFromFile(file, currentSemester.Value.Id, cancellationToken);
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

    public async Task<OperationResult<Dictionary<DateTime, List<DefendCapstoneCalendarResponse>>>>
        GetDefendCalendersByCouncilMember(CancellationToken cancellationToken)
    {
        var result = new Dictionary<DateTime, List<DefendCapstoneCalendarResponse>>();

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
            include: x => x.AsSplitQuery()
                .Include(x => x.DefendCapstoneProjectMemberCouncils)
                .ThenInclude(x => x.Supervisor)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group),
            cancellationToken);

        if (calendars is null || calendars.Count == 0)
            return result;

        foreach (var calendar in calendars.GroupBy(x => x.DefenseDate).OrderBy(x => x.Key))
        {
            result[calendar.Key] = calendar.Select(x => new DefendCapstoneCalendarResponse
                {
                    Id = x.Id,
                    TopicId = x.TopicId,
                    GroupId = x.Topic.Group.Id,
                    DefenseDate = x.DefenseDate,
                    DefendAttempt = x.DefendAttempt,
                    Location = x.Location,
                    Slot = x.Slot,
                    CampusId = x.CampusId,
                    SemesterId = x.SemesterId,
                    CapstoneId = x.CapstoneId,
                    TopicCode = x.TopicCode,
                    Status = x.Status.ToString(),
                    GroupCode = x.Topic.Group.GroupCode,
                    CouncilMembers = x.DefendCapstoneProjectMemberCouncils.Select(x =>
                        new DefendCapstoneCouncilMemberDto
                        {
                            Id = x.Id,
                            IsPresident = x.IsPresident,
                            IsSecretary = x.IsSecretary,
                            SupervisorId = x.SupervisorId,
                            SupervisorName = x.Supervisor.FullName,
                            DefendCapstoneProjectInformationCalendarId =
                                x.DefendCapstoneProjectInformationCalendarId
                        }).ToList(),
                })
                .OrderBy(x => x.Slot)
                .ToList();
        }

        return result;
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
                include: x => x.AsSplitQuery()
                    .Include(x => x.DefendCapstoneProjectMemberCouncils)
                    .Include(x => x.Topic)
                    .ThenInclude(x => x.Group),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(calendar);

            if (!calendar.DefendCapstoneProjectMemberCouncils
                    .Any(x => x.SupervisorId == currentUser.UserCode && (x.IsPresident || x.IsSecretary)))
                return OperationResult.Failure(new Error("DefendCapstone.Error",
                    "Only who has the permission can do this action."));

            var key =
                $"{calendar.CampusId}/{calendar.SemesterId}/{calendar.Topic.CapstoneId}/{calendar.TopicCode}/{calendar.DefendAttempt}/{calendar.Topic.Group.GroupCode}";

            await unitOfWork.BeginTransactionAsync(cancellationToken);

            calendar.IsUploadedThesisMinute = true;

            defendCapstoneCalendarRepository.Update(calendar);

            if ((await documentsService.CreateThesisDocument(request.File, key, cancellationToken)).IsFailure)
                throw new InvalidOperationException("Upload thesis fail.");

            await unitOfWork.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Can not upload the thesis Id {Thesis} with error: {Message}",
                request.DefendCapstoneCalendarId, ex.Message);

            await unitOfWork.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("DefendCapstone.Error", "Can not upload thesis document."));
        }
    }

    public async Task<OperationResult<string>> PresentThesisForTopicResignedUrl(Guid calendarId,
        CancellationToken cancellationToken)
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

        var key =
            $"{calendar.CampusId}/{calendar.SemesterId}/{calendar.Topic.CapstoneId}/{calendar.TopicCode}/{calendar.DefendAttempt}";

        return await documentsService.PresentThesisCouncilMeetingMinutesForTopicPresignedUrl(key);
    }

    public async Task<OperationResult> UpdateStatusOfGroupAfterDefend(
        UpdateGroupDecisionStatusByPresidentRequest request,
        CancellationToken cancellationToken)
    {
        var calendar = await defendCapstoneCalendarRepository.GetAsync(
            x => x.Id == request.CalendarId,
            include: x => x.AsSplitQuery()
                .Include(x => x.DefendCapstoneProjectMemberCouncils)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(calendar);

        if (!calendar.IsUploadedThesisMinute)
            return OperationResult.Failure(new Error("DefendCapstone.Error",
                "The thesis file of that was not uploaded."));

        if (!calendar.DefendCapstoneProjectMemberCouncils
                .Any(x => x.SupervisorId == currentUser.UserCode && x.IsPresident))
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You can not get this thesis because you are not in the Council."));

        // Update status of group
        return await groupService.UpdateGroupDecisionByPresidentIdAsync(calendar.Topic.Group.Id, calendar.Id,
            request.IsReDefendCapstoneProject);
    }

    public async Task<OperationResult<DefendCapstoneCalendarDetailResponse>> GetDefendCapstoneCalendarByIdAsync(
        Guid defendCapstoneCalendarId)
    {
        var calendar = await defendCapstoneCalendarRepository.GetAsync(
            x => x.Id == defendCapstoneCalendarId,
            include: x => x.AsSplitQuery()
                .Include(x => x.DefendCapstoneProjectMemberCouncils)
                .ThenInclude(x => x.Supervisor)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group)
                .ThenInclude(x => x.Supervisor));

        if (calendar is null)
            return OperationResult.Failure<DefendCapstoneCalendarDetailResponse>(new Error("Error.NotFound",
                "Defend capstone calendar not found"));

        if (calendar.DefendCapstoneProjectMemberCouncils.All(x => x.SupervisorId != currentUser.UserCode))
            return OperationResult.Failure<DefendCapstoneCalendarDetailResponse>(new Error("Error.NotFound",
                "Can not get this calendar because you are not in the Council."));

        var response = new DefendCapstoneCalendarDetailResponse()
        {
            Id = calendar.Id,
            TopicId = calendar.TopicId,
            GroupId = calendar.Topic.Group.Id,
            DefenseDate = calendar.DefenseDate,
            DefendAttempt = calendar.DefendAttempt,
            Location = calendar.Location,
            Slot = calendar.Slot,
            CampusId = calendar.CampusId,
            SemesterId = calendar.SemesterId,
            TopicCode = calendar.TopicCode,
            GroupCode = calendar.Topic.Group.GroupCode,
            CouncilMembers = calendar.DefendCapstoneProjectMemberCouncils.Select(x =>
                new DefendCapstoneCouncilMemberDto
                {
                    Id = x.Id,
                    IsPresident = x.IsPresident,
                    IsSecretary = x.IsSecretary,
                    SupervisorId = x.SupervisorId,
                    SupervisorName = x.Supervisor.FullName,
                    DefendCapstoneProjectInformationCalendarId = x.DefendCapstoneProjectInformationCalendarId
                }).ToList(),
            CapstoneId = calendar.CapstoneId,
            SupervisorId = calendar.Topic.Group.SupervisorId!,
            SupervisorName = calendar.Topic.Group.Supervisor!.FullName,
            Abbreviation = calendar.Topic.Abbreviation,
            Description = calendar.Topic.Description,
            TopicEngName = calendar.Topic.EnglishName,
            TopicVietName = calendar.Topic.VietnameseName
        };

        return OperationResult.Success(response);
    }

    private async Task<List<DefendCapstoneProjectInformationCalendar>> ParseDefendCapstoneCalendarsFromFile(
        IFormFile file, string currentSemesterId, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        using var wb = new XLWorkbook(stream);
        IXLWorksheet workSheet = wb.Worksheet(1);

        var defendCalendars = new List<DefendCapstoneProjectInformationCalendar>();
        var memberColumn = 8;
        var memberInfoList = new List<string>();

        var existingDefendCalendars = await defendCapstoneCalendarRepository.GetAllAsync();
        var attempt = existingDefendCalendars.Any() ? existingDefendCalendars.Max(rc => rc.DefendAttempt) + 1 : 1;

        if (attempt > systemConfigService.GetSystemConfiguration().MaxAttemptTimesToDefendCapstone)
        {
            throw new InvalidOperationException("You have reached the maximum number of attempts to defend capstone.");
        }


        foreach (var row in workSheet.Rows().Skip(5))
        {
            var topicCode = row.Cell(2).GetValue<string>();
            if (string.IsNullOrEmpty(topicCode))
            {
                break;
            }

            var topicResult = await topicService.GetTopicByCode(topicCode, cancellationToken);
            if (topicResult.IsFailure ||
                topicResult.Value.Status != TopicStatus.Approved ||
                topicResult.Value.IsAssignedToGroup == false ||
                topicResult.Value.CapstoneId != currentUser.CapstoneId ||
                topicResult.Value.CampusId != currentUser.CampusId)
                throw new InvalidOperationException("Topic is not available for defend phase.");

            var presidentAndSecretary = await GetPresidentAndSecretaryAsync(row, topicResult.Value);
            var defendCapstoneProjectCalendarDetail = GetDefendCapstoneProjectCalendarDetail(row);

            for (; memberColumn <= row.Cells().Count(); memberColumn++)
            {
                if (!string.IsNullOrEmpty(workSheet.Cell(5, memberColumn).GetValue<string>()))
                {
                    var memberInfo = await GetMemberInformationAsync(row, topicResult.Value, memberColumn);

                    memberInfoList.Add(memberInfo.Id);
                    // check if member information is duplicate value with president or secretary
                    if (IsMemberInformationValid(memberInfoList, presidentAndSecretary.Item1.Id,
                            presidentAndSecretary.Item2.Id))
                        throw new InvalidOperationException(
                            "Member information is duplicate value with president or secretary.");
                }
            }

            var defendCalendar = CreateDefendCalendar(
                topicResult.Value,
                currentUser.CampusId,
                currentSemesterId,
                currentUser.CapstoneId,
                defendCapstoneProjectCalendarDetail,
                attempt,
                presidentAndSecretary,
                memberInfoList);
            defendCalendars.Add(defendCalendar);
        }

        return defendCalendars;
    }

    private static DefendCapstoneProjectInformationCalendar CreateDefendCalendar(
        Topic topic,
        string currentUserCampusId,
        string currentSemesterId,
        string currentCapstoneId,
        (DateTime, int, string) defendCapstoneProjectCalendarDetail,
        int attempt,
        (SupervisorResponseDTO, SupervisorResponseDTO) presidentAndSecretary,
        List<string> memberInfoList)
    {
        var defendCalendar = new DefendCapstoneProjectInformationCalendar
        {
            Id = Guid.NewGuid(),
            TopicId = topic.Id,
            TopicCode = topic.Code!,
            CampusId = currentUserCampusId,
            SemesterId = currentSemesterId,
            CapstoneId = currentCapstoneId,
            DefenseDate = defendCapstoneProjectCalendarDetail.Item1,
            Slot = defendCapstoneProjectCalendarDetail.Item2,
            Location = defendCapstoneProjectCalendarDetail.Item3,
            DefendAttempt = attempt
        };

        AddPresidentToDefendCalendar(defendCalendar, presidentAndSecretary.Item1);
        AddSecretaryToDefendCalendar(defendCalendar, presidentAndSecretary.Item2);
        AddMembersToDefendCalendar(defendCalendar, memberInfoList);
        return defendCalendar;
    }

    private static void AddPresidentToDefendCalendar(
        DefendCapstoneProjectInformationCalendar defendCalendar,
        SupervisorResponseDTO president)
    {
        defendCalendar.DefendCapstoneProjectMemberCouncils.Add(new DefendCapstoneProjectCouncilMember
        {
            Id = Guid.NewGuid(),
            DefendCapstoneProjectInformationCalendarId = defendCalendar.Id,
            IsPresident = true,
            IsSecretary = false,
            SupervisorId = president.Id
        });
    }

    private static void AddSecretaryToDefendCalendar(
        DefendCapstoneProjectInformationCalendar defendCalendar,
        SupervisorResponseDTO secretary)
    {
        defendCalendar.DefendCapstoneProjectMemberCouncils.Add(new DefendCapstoneProjectCouncilMember
        {
            Id = Guid.NewGuid(),
            DefendCapstoneProjectInformationCalendarId = defendCalendar.Id,
            IsPresident = false,
            IsSecretary = true,
            SupervisorId = secretary.Id
        });
    }

    private static void AddMembersToDefendCalendar(
        DefendCapstoneProjectInformationCalendar defendCalendar,
        List<string> memberInfoList)
    {
        memberInfoList.Select(x => new DefendCapstoneProjectCouncilMember
        {
            Id = Guid.NewGuid(),
            SupervisorId = x,
            IsPresident = false,
            IsSecretary = false,
            DefendCapstoneProjectInformationCalendarId = defendCalendar.Id
        }).ToList().ForEach(x => defendCalendar.DefendCapstoneProjectMemberCouncils.Add(x));
    }

    private (DateTime, int, string) GetDefendCapstoneProjectCalendarDetail(IXLRow row)
    {
        var reviewDate = row.Cell(3).GetValue<string>();
        var slot = row.Cell(4).GetValue<string>();
        var room = row.Cell(5).GetValue<string>();

        if (string.IsNullOrEmpty(reviewDate) || string.IsNullOrEmpty(slot) || string.IsNullOrEmpty(room))
        {
            logger.LogError("import defend capstone calendar failed with message: review date, slot or room is empty!");
            throw new InvalidOperationException("Invalid defend capstone calendar details");
        }

        return (DateTime.Parse(reviewDate), int.Parse(slot), room);
    }

    private static bool IsMemberInformationValid(List<string> memberInfoList, string presidentId, string secretaryId)
    {
        return memberInfoList.Exists(x => x == presidentId || x == secretaryId);
    }

    private async Task<SupervisorResponseDTO> GetMemberInformationAsync(IXLRow row, Topic topic, int memberColumn)
    {
        var memberInfo =
            await supervisorService.GetSupervisorByIdAsync(row.Cell(memberColumn).GetValue<string>());

        return memberInfo.IsFailure
            ? throw new InvalidOperationException("Member information is invalid ")
            : IsCouncilMemberIsMainSupervisor(topic, memberInfo.Value.Id)
                ? throw new InvalidOperationException("Can not assign this supervisor for their own topic.")
                : memberInfo.Value;
    }

    private async Task<(SupervisorResponseDTO, SupervisorResponseDTO)> GetPresidentAndSecretaryAsync(IXLRow row,
        Topic topic)
    {
        var presidentInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(6).GetValue<string>());
        if (presidentInformation.IsFailure)
            throw new InvalidOperationException("President is not available for defend phase.");

        var secretaryInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(7).GetValue<string>());
        if (secretaryInformation.IsFailure)
            throw new InvalidOperationException("Secretary is not available for defend phase.");

        return IsCouncilMemberIsMainSupervisor(topic, presidentInformation.Value.Id) ||
               IsCouncilMemberIsMainSupervisor(topic, secretaryInformation.Value.Id)
            ? throw new InvalidOperationException("Can not assign this supervisor for their own topic.")
            : (presidentInformation.Value, secretaryInformation.Value);
    }

    private static bool IsCouncilMemberIsMainSupervisor(Topic topic, string councilMemberId)
    {
        return topic.MainSupervisorId == councilMemberId;
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase) ||
                file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase));
    }

    public async Task<OperationResult<Dictionary<DateTime, List<DefendCapstoneCalendarResponse>>>>
        GetDefendCalendersByManager(CancellationToken cancellationToken)
    {
        var result = new Dictionary<DateTime, List<DefendCapstoneCalendarResponse>>();

        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<Dictionary<DateTime, List<DefendCapstoneCalendarResponse>>>(currentSemester
                .Error);

        var calendars = await defendCapstoneCalendarRepository.FindAsync(
            x => x.CampusId == currentUser.CampusId &&
                 x.CapstoneId == currentUser.CapstoneId &&
                 x.SemesterId == currentSemester.Value.Id,
            include: x => x.AsSplitQuery()
                .Include(x => x.DefendCapstoneProjectMemberCouncils)
                .ThenInclude(x => x.Supervisor)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group),
            cancellationToken);

        if (calendars is null || calendars.Count == 0)
            return result;

        foreach (var calendar in calendars.GroupBy(x => x.DefenseDate).OrderBy(x => x.Key))
        {
            result[calendar.Key] = calendar.Select(x => new DefendCapstoneCalendarResponse
                {
                    Id = x.Id,
                    TopicId = x.TopicId,
                    GroupId = x.Topic.Group.Id,
                    DefenseDate = x.DefenseDate,
                    DefendAttempt = x.DefendAttempt,
                    Location = x.Location,
                    Slot = x.Slot,
                    CampusId = x.CampusId,
                    SemesterId = x.SemesterId,
                    CapstoneId = x.CapstoneId,
                    TopicCode = x.TopicCode,
                    GroupCode = x.Topic.Group.GroupCode,
                    CouncilMembers = x.DefendCapstoneProjectMemberCouncils.Select(x =>
                        new DefendCapstoneCouncilMemberDto
                        {
                            Id = x.Id,
                            IsPresident = x.IsPresident,
                            IsSecretary = x.IsSecretary,
                            SupervisorId = x.SupervisorId,
                            SupervisorName = x.Supervisor.FullName,
                            DefendCapstoneProjectInformationCalendarId =
                                x.DefendCapstoneProjectInformationCalendarId
                        }).ToList(),
                })
                .OrderBy(x => x.Slot)
                .ToList();
        }

        return result;
    }
}
