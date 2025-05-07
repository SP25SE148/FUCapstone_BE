using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
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
    ITimeConfigurationService timeConfigurationService,
    IRepository<Group> groupRepository,
    IUnitOfWork<FucDbContext> unitOfWork,
    IIntegrationEventLogService integrationEventLogService,
    ITopicService topicService,
    IGroupService groupService,
    ISupervisorService supervisorService,
    ISystemConfigurationService systemConfigService,
    IDocumentsService documentsService) : IDefendCapstoneService
{
    public async Task<OperationResult> UploadDefendCapstoneProjectCalendar(IFormFile file,
        string semesterId,
        CancellationToken cancellationToken)
    {
        if (!IsValidFile(file))
        {
            return OperationResult.Failure(new Error("DefendCapstoneProjectCalendar.Error", "Invalid form file."));
        }

        try
        {
            // get current semester 
            var timeConfiguration = await timeConfigurationService.GetTimeConfigurationBySemesterId(semesterId);
            if (timeConfiguration is null)
                return OperationResult.Failure(new Error("Error.UploadFailed",
                    "Import calendar failed cause its not in the valid time configuration !"));

            if (
                timeConfiguration.Value.IsActived &&
                (timeConfiguration.Value.DefendCapstoneProjectDate > DateTime.Now
                 || timeConfiguration.Value.DefendCapstoneProjectExpiredDate < DateTime.Now))
                return OperationResult.Failure<Guid>(new Error("CreateFailed",
                    "Must import the defend calendar for group on available time. The time that you can import the defend calendar file is from " +
                    timeConfiguration.Value.DefendCapstoneProjectDate + " to " +
                    timeConfiguration.Value.DefendCapstoneProjectExpiredDate));

            var defendCalendars =
                await ParseDefendCapstoneCalendarsFromFile(file, timeConfiguration.Value.SemesterId, cancellationToken);

            defendCapstoneCalendarRepository.InsertRange(defendCalendars);

            // send review calendar created event

            var calendarCreatedDetails = new List<CalendarCreatedDetail>();

            foreach (var defendCalendar in defendCalendars)
            {
                var calendarCreatedDetail = new CalendarCreatedDetail()
                {
                    CalendarId = defendCalendar.Id,
                    Users = defendCalendar.DefendCapstoneProjectMemberCouncils.Select(x => x.SupervisorId).ToList(),
                    StartDate = defendCalendar.DefenseDate,
                    Type = nameof(DefendCapstoneProjectInformationCalendar)
                };
                var group = await groupService.GetGroupByTopicIdAsync(defendCalendar.TopicId);

                calendarCreatedDetail.Users.AddRange(group.Value.GroupMemberList.Select(x => x.StudentId));
                calendarCreatedDetails.Add(calendarCreatedDetail);
            }

            integrationEventLogService.SendEvent(new CalendarCreatedEvent
            {
                Details = calendarCreatedDetails
            });

            await unitOfWork.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("import review failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.ImportFailed", e.Message));
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
                    Time = x.Time,
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
                .OrderBy(x => x.Time)
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
            x => x.Id == calendarId && x.IsUploadedThesisMinute,
            include: x => x.Include(x => x.DefendCapstoneProjectMemberCouncils)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(calendar);

        if (currentUser.Role == UserRoles.Student)
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You do not have the permission."));

        if (currentUser.Role == UserRoles.Supervisor &&
            !calendar.DefendCapstoneProjectMemberCouncils.Any(x => x.SupervisorId == currentUser.UserCode))
            return OperationResult.Failure<string>(new Error("DefendCapstone.Error",
                "You can not get this thesis because you are not in the Council."));

        var key =
            $"{calendar.CampusId}/{calendar.SemesterId}/{calendar.Topic.CapstoneId}/{calendar.TopicCode}/{calendar.DefendAttempt}/{calendar.Topic.Group.GroupCode}";

        return await documentsService.PresentThesisCouncilMeetingMinutesForTopicPresignedUrl(key);
    }

    public async Task<OperationResult> UpdateDefendCalendarStatus(UpdateDefendCalendarStatusRequest request)
    {
        var calendar = await defendCapstoneCalendarRepository
            .GetAsync(x => x.Id == request.Id,
                true);

        if (calendar is null)
            return OperationResult.Failure(Error.NullValue);
        var group = await groupRepository.GetAsync(
            x => x.TopicId == calendar.TopicId,
            true,
            include: x => x.Include(x => x.DefendCapstoneProjectDecision));

        calendar.Status = request.Status;

        if (request.Status == DefendCapstoneProjectCalendarStatus.Done)
        {
            group!.IsReDefendCapstoneProject = request.IsReDefend;

            if (request.IsReDefend)
            {
                group.DefendCapstoneProjectDecision.Decision = DecisionStatus.Revised_for_the_second_defense;
            }
            else
            {
                group.Status = GroupStatus.Completed;
            }
        }

        await unitOfWork.SaveChangesAsync();
        return OperationResult.Success();
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
            Time = calendar.Time,
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

    public async Task<OperationResult<IEnumerable<DefendCapstoneCalendarDetailResponse>>>
        GetDefendCapstoneCalendarByGroupSelf()
    {
        var group = await groupService.GetGroupInformationByGroupSelfId();

        if (group.IsFailure)
            return OperationResult.Failure<IEnumerable<DefendCapstoneCalendarDetailResponse>>(Error.NullValue);

        if (!Guid.TryParse(group.Value.TopicResponse?.Id, out Guid topicId))
        {
            return new List<DefendCapstoneCalendarDetailResponse>();
        }

        var defendCapstoneCalendars =
            await defendCapstoneCalendarRepository.FindAsync(
                dc => dc.TopicId == topicId,
                include: x => x.AsSplitQuery()
                    .Include(x => x.DefendCapstoneProjectMemberCouncils)
                    .ThenInclude(x => x.Supervisor)
                    .Include(x => x.Topic)
                    .ThenInclude(x => x.Group)
                    .ThenInclude(x => x.Supervisor),
                orderBy: dc => dc.OrderBy(dc => dc.CreatedBy),
                selector: calendar => new DefendCapstoneCalendarDetailResponse()
                {
                    Id = calendar.Id,
                    TopicId = calendar.TopicId,
                    GroupId = calendar.Topic.Group.Id,
                    DefenseDate = calendar.DefenseDate,
                    DefendAttempt = calendar.DefendAttempt,
                    Location = calendar.Location,
                    Time = calendar.Time,
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
                    TopicVietName = calendar.Topic.VietnameseName,
                    Status = calendar.Status.ToString()
                });

        return defendCapstoneCalendars.Any()
            ? defendCapstoneCalendars.ToList()
            : OperationResult.Failure<IEnumerable<DefendCapstoneCalendarDetailResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<DefendCapstoneResultResponse>>>
        GetDefendCapstoneResultByGroupId(Guid groupId)
    {
        var topic = await topicService.GetTopicByGroupIdAsync(groupId);

        if (topic.IsFailure)
            return OperationResult.Failure<IEnumerable<DefendCapstoneResultResponse>>(new Error("Error.NotFound",
                "Group not found"));

        if (currentUser.Role == UserRoles.Student)
        {
            if (topic.Value.Group.GroupMembers.All(gm => gm.StudentId != currentUser.UserCode))
            {
                return OperationResult.Failure<IEnumerable<DefendCapstoneResultResponse>>(new Error("Error.NotFound",
                    "You are not in this group"));
            }
        }
        else if (currentUser.Role == UserRoles.Supervisor)
        {
            var isMainOrCoSupervisor = topic.Value.MainSupervisorId == currentUser.UserCode ||
                                       topic.Value.CoSupervisors.Any(c => c.SupervisorId == currentUser.UserCode);

            if (!isMainOrCoSupervisor)
            {
                return OperationResult.Failure<IEnumerable<DefendCapstoneResultResponse>>(new Error("Error.NotFound",
                    "You are not the main or co-supervisor of this group"));
            }
        }
        else
        {
            return OperationResult.Failure<IEnumerable<DefendCapstoneResultResponse>>(new Error("Error.Unauthorized",
                "You are not authorized to view this group’s defense result"));
        }

        var defendCapstoneResults = await defendCapstoneCalendarRepository.FindAsync(dc => dc.TopicId == topic.Value.Id,
            x => x.AsSplitQuery()
                .Include(x => x.DefendCapstoneProjectMemberCouncils)
                .ThenInclude(x => x.Supervisor)
                .Include(x => x.Topic)
                .ThenInclude(x => x.Group)
                .ThenInclude(x => x.Supervisor),
            orderBy: dc => dc.OrderBy(dc => dc.CreatedBy),
            selector: calendar => new DefendCapstoneResultResponse
            {
                Id = calendar.Id,
                TopicId = calendar.TopicId,
                GroupId = calendar.Topic.Group.Id,
                DefenseDate = calendar.DefenseDate,
                DefendAttempt = calendar.DefendAttempt,
                Location = calendar.Location,
                Time = calendar.Time,
                CampusId = calendar.CampusId,
                SemesterId = calendar.SemesterId,
                TopicCode = calendar.TopicCode,
                GroupCode = calendar.Topic.Group.GroupCode,
                CapstoneId = calendar.CapstoneId,
                Status = calendar.Status.ToString(),
                GroupStatus = calendar.Topic.Group.Status.ToString(),
                IsReDefendCapstone = calendar.Topic.Group.IsReDefendCapstoneProject,
                CouncilMembers = calendar.DefendCapstoneProjectMemberCouncils.Select(x =>
                    new DefendCapstoneCouncilMemberDto
                    {
                        Id = x.Id,
                        IsPresident = x.IsPresident,
                        IsSecretary = x.IsSecretary,
                        SupervisorId = x.SupervisorId,
                        SupervisorName = x.Supervisor.FullName,
                        DefendCapstoneProjectInformationCalendarId = x.DefendCapstoneProjectInformationCalendarId
                    }).ToList()
            });
        return defendCapstoneResults.Any() ? defendCapstoneResults.ToList() : null;
    }

    private async Task<List<DefendCapstoneProjectInformationCalendar>> ParseDefendCapstoneCalendarsFromFile(
        IFormFile file, string semesterId, CancellationToken cancellationToken)
    {
        var stream = new MemoryStream();
        await file.CopyToAsync(stream, cancellationToken);
        using var wb = new XLWorkbook(stream);
        IXLWorksheet workSheet = wb.Worksheet(1);

        var defendCalendars = new List<DefendCapstoneProjectInformationCalendar>();
        var memberInfoList = new List<string>();

        var existingDefendCalendars = await defendCapstoneCalendarRepository.GetAllAsync();
        if (existingDefendCalendars.Any(x => x.Status != DefendCapstoneProjectCalendarStatus.Done))
            throw new Exception("The current defend capstone project calendars has not been completed.");

        var attempt = existingDefendCalendars.Any() ? existingDefendCalendars.Max(rc => rc.DefendAttempt) + 1 : 1;

        if (attempt > systemConfigService.GetSystemConfiguration().MaxAttemptTimesToDefendCapstone)
        {
            throw new InvalidOperationException("You have reached the maximum number of attempts to defend capstone.");
        }


        foreach (var row in workSheet.Rows().Skip(5))
        {
            var topicCode = row.Cell(3).GetValue<string>();
            var groupCode = row.Cell(2).GetValue<string>();
            if (string.IsNullOrEmpty(topicCode) || string.IsNullOrEmpty(groupCode))
            {
                break;
            }

            var groupResult = await groupRepository.GetAsync(g => g.GroupCode == groupCode,
                include: x => x.Include(x => x.DefendCapstoneProjectDecision),
                cancellationToken: cancellationToken);


            var topicResult = await topicService.GetTopicByCode(topicCode, cancellationToken);

            if (topicResult.IsFailure ||
                topicResult.Value.Status != TopicStatus.Approved ||
                topicResult.Value.IsAssignedToGroup == false ||
                topicResult.Value.CapstoneId != currentUser.CapstoneId ||
                topicResult.Value.CampusId != currentUser.CampusId ||
                groupResult is null ||
                groupResult.TopicId != topicResult.Value.Id ||
                !IsDecisionStatusValidForAttempt(groupResult.DefendCapstoneProjectDecision.Decision, attempt))
                throw new InvalidOperationException("Topic is not available for defend phase.");

            var presidentAndSecretary = await GetPresidentAndSecretaryAsync(row, topicResult.Value);
            var defendCapstoneProjectCalendarDetail = GetDefendCapstoneProjectCalendarDetail(row);

            // get member information
            var memberColumn = 11;
            for (; memberColumn <= row.Cells().Count(); memberColumn++)
            {
                if (string.IsNullOrEmpty(workSheet.Cell(5, memberColumn).GetValue<string>()))
                {
                    break;
                }

                var memberInfo = await GetMemberInformationAsync(row, topicResult.Value, memberColumn);

                memberInfoList.Add(memberInfo.Id);
                // check if member information is duplicate value with president or secretary
                if (IsMemberInformationValid(memberInfoList, presidentAndSecretary.Item1.Id,
                        presidentAndSecretary.Item2.Id))
                    throw new InvalidOperationException(
                        "Member information is duplicate value with president or secretary.");
            }

            var defendCalendar = CreateDefendCalendar(
                topicResult.Value,
                currentUser.CampusId,
                semesterId,
                currentUser.CapstoneId,
                defendCapstoneProjectCalendarDetail,
                attempt,
                presidentAndSecretary,
                memberInfoList);
            defendCalendars.Add(defendCalendar);
        }

        return defendCalendars;
    }

    private static bool IsDecisionStatusValidForAttempt(DecisionStatus status, int attempt)
    {
        return (status, attempt) switch
        {
            (DecisionStatus.Agree_to_defense, 1) => true,
            (DecisionStatus.Revised_for_the_second_defense, 2) => true,
            (DecisionStatus.Disagree_to_defense, _) => false,
            _ => false
        };
    }

    private static DefendCapstoneProjectInformationCalendar CreateDefendCalendar(
        Topic topic,
        string currentUserCampusId,
        string semesterId,
        string currentCapstoneId,
        (DateTime, string, string) defendCapstoneProjectCalendarDetail,
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
            SemesterId = semesterId,
            CapstoneId = currentCapstoneId,
            DefenseDate = defendCapstoneProjectCalendarDetail.Item1,
            Time = defendCapstoneProjectCalendarDetail.Item2,
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

    private (DateTime, string, string) GetDefendCapstoneProjectCalendarDetail(IXLRow row)
    {
        var reviewDate = row.Cell(6).GetValue<string>();
        var time = row.Cell(7).GetValue<string>();
        var room = row.Cell(8).GetValue<string>();

        if (string.IsNullOrEmpty(reviewDate) || string.IsNullOrEmpty(time) || string.IsNullOrEmpty(room))
        {
            logger.LogError("import defend capstone calendar failed with message: review date, slot or room is empty!");
            throw new InvalidOperationException("Invalid defend capstone calendar details");
        }

        // Validate review date
        if (DateTime.TryParse(reviewDate, out var date) && date <= DateTime.Now)
        {
            throw new Exception("Defend date must be in the future and in correct format");
        }

        return (date, time, room);
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
        var presidentInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(9).GetValue<string>());
        if (presidentInformation.IsFailure)
            throw new InvalidOperationException("President is not available for defend phase.");

        var secretaryInformation = await supervisorService.GetSupervisorByIdAsync(row.Cell(10).GetValue<string>());
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


        var calendars = await defendCapstoneCalendarRepository.FindAsync(
            x => x.CampusId == currentUser.CampusId &&
                 x.CapstoneId == currentUser.CapstoneId,
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
                    Status = x.Status.ToString(),
                    TopicId = x.TopicId,
                    GroupId = x.Topic.Group.Id,
                    DefenseDate = x.DefenseDate,
                    DefendAttempt = x.DefendAttempt,
                    Location = x.Location,
                    Time = x.Time,
                    IsUploadedThesisMinute = x.IsUploadedThesisMinute,
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
                .OrderBy(x => x.Time)
                .ToList();
        }

        return result;
    }
}
