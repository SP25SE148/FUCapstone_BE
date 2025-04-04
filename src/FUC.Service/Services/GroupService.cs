using System.Linq.Expressions;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
using ClosedXML.Excel;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Contracts;
using FUC.Common.Helpers;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.DTOs.TopicRequestDTO;
using FUC.Service.Extensions.Options;
using FUC.Service.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Group = FUC.Data.Entities.Group;

namespace FUC.Service.Services;

public class GroupService(
    IUnitOfWork<FucDbContext> uow,
    ISemesterService semesterService,
    IMapper mapper,
    ILogger<GroupService> logger,
    IIntegrationEventLogService integrationEventLogService,
    ICurrentUser currentUser,
    IRepository<GroupMember> groupMemberRepository,
    IRepository<TopicRequest> topicRequestRepository,
    IRepository<ProjectProgress> projectProgressRepository,
    IRepository<FucTask> fucTaskRepository,
    IRepository<WeeklyEvaluation> weeklyEvaluationRepository,
    IRepository<Student> studentRepository,
    IRepository<Group> groupRepository,
    IRepository<CoSupervisor> coSupervisorRepository,
    ITopicService topicService,
    IRepository<DefendCapstoneProjectCouncilMember> defendCapstoneProjectCouncilMemberRepository,
    ICapstoneService capstoneService,
    IS3Service s3Service,
    IDocumentsService documentsService,
    IRepository<DefendCapstoneProjectDecision> defendCapstoneDecisionRepository,
    ISystemConfigurationService systemConfigService,
    ITimeConfigurationService timeConfigurationService,
    S3BucketConfiguration s3BucketConfiguration) : IGroupService
{
    private const int IndexStartProgressingRow = 6;

    public async Task<OperationResult<Guid>> CreateGroupAsync()
    {
        OperationResult<Semester> currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
            return OperationResult.Failure<Guid>(new Error("Error.SemesterIsNotGoingOn",
                "The current semester is not going on"));

        Student? leader = await studentRepository.GetAsync(
            predicate: s =>
                s.Id == currentUser.UserCode &&
                !s.IsDeleted &&
                s.Status.Equals(StudentStatus.InProgress),
            include: s =>
                s.Include(s => s.GroupMembers)
                    .ThenInclude(gm => gm.Group)
                    .Include(s => s.Capstone),
            orderBy: default,
            cancellationToken: default);

        // Check if leader is null 
        if (leader is null)
            return OperationResult.Failure<Guid>(Error.NullValue);

        // Check if Leader is eligible to create group 
        if (leader.GroupMembers.Count > 0 &&
            leader.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
            return OperationResult.Failure<Guid>(new Error("Error.InEligible", "Leader is ineligible to create group"));

        try
        {
            // Create group, group member for leader
            await uow.BeginTransactionAsync();
            var newGroup = new Group()
            {
                Id = Guid.NewGuid(),
                CampusId = leader.CampusId,
                CapstoneId = leader.CapstoneId,
                MajorId = leader.MajorId,
                SemesterId = currentSemester.Value.Id,
            };

            groupRepository.Insert(newGroup);

            groupMemberRepository.Insert(new()
            {
                GroupId = newGroup.Id,
                StudentId = leader.Id,
                IsLeader = true,
                Status = GroupMemberStatus.Accepted
            });
            var groupMembers = await groupMemberRepository.FindAsync(gm => gm.StudentId == currentUser.UserCode);
            if (groupMembers.Count > 0)
            {
                foreach (var groupMember in groupMembers)
                {
                    groupMember.Status = GroupMemberStatus.Rejected;
                    groupMemberRepository.Update(groupMember);
                }
            }

            await uow.CommitAsync();


            return newGroup.Id;
        }
        catch (Exception e)
        {
            logger.LogError("Create group failed with message: {Message}", e.Message);
            return OperationResult.Failure<Guid>(new Error("Error.CreateGroupFailed", "Create group failed"));
        }
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupAsync()
    {
        var groups = await groupRepository.GetAllAsync(
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());
        foreach (var group in groups)
        {
            group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        }

        return groups.Count > 0
            ? groups
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupBySemesterIdAsync(string semesterId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.SemesterId == semesterId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse()
        );
        foreach (var group in groups)
        {
            group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        }

        return groups.Count > 0
            ? groups.ToList()
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByMajorIdAsync(string majorId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.MajorId == majorId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());
        foreach (var group in groups)
        {
            group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        }

        return groups.Count > 0
            ? groups.ToList()
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCapstoneIdAsync(string capstoneId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.CapstoneId == capstoneId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());
        foreach (var group in groups)
        {
            group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        }

        return groups.Count > 0
            ? groups.ToList()
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCampusIdAsync(string campusId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.CampusId == campusId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());
        foreach (var group in groups)
        {
            group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        }

        return groups.Count > 0
            ? groups.ToList()
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetPendingGroupsForStudentJoin(
        CancellationToken cancellationToken)
    {
        var capstoneResult = await capstoneService.GetCapstoneByIdAsync(currentUser.CapstoneId);

        if (capstoneResult.IsFailure)
            return OperationResult.Failure<IEnumerable<GroupResponse>>(capstoneResult.Error);

        var groups = await groupRepository.FindAsync(
            g => g.CampusId == currentUser.CampusId &&
                 g.MajorId == currentUser.MajorId &&
                 g.CapstoneId == currentUser.CapstoneId &&
                 g.Status == GroupStatus.Pending &&
                 g.GroupMembers.Count(
                     x => x.Status == GroupMemberStatus.Accepted ||
                          x.Status == GroupMemberStatus.UnderReview) <= capstoneResult.Value.MaxMember,
            include: g => g.Include(g => g.GroupMembers.Where(s => s.Status == GroupMemberStatus.Accepted))
                .ThenInclude(gm => gm.Student),
            orderBy: x => x.OrderBy(g => g.GroupCode),
            CreateSelectorForGroupResponse(),
            cancellationToken);

        return groups.Count > 0
            ? groups.ToList()
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<GroupResponse>> GetGroupByIdAsync(Guid id)
    {
        var group = await groupRepository.GetAsync(g => g.Id == id,
            CreateSelectorForGroupResponse(),
            g => g.AsSplitQuery()
                .Include(g => g.GroupMembers.Where(gm => gm.Status == GroupMemberStatus.Accepted))
                .ThenInclude(gm => gm.Student)
                .Include(g => g.Supervisor)
                .Include(x => x.Topic)
                .ThenInclude(t => t.CoSupervisors)
                .Include(g => g.Capstone));
        if (group is null)
            return OperationResult.Failure<GroupResponse>(Error.NullValue);
        group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        return group;
    }

    public async Task<OperationResult<GroupResponse>> GetGroupByGroupCodeAsync(string groupCode)
    {
        var groupResponse = await groupRepository.GetAsync(
            g => g.GroupCode == groupCode,
            CreateSelectorForGroupResponse(),
            CreateIncludeForGroupResponse());

        return groupResponse != null
            ? OperationResult.Success(groupResponse)
            : OperationResult.Failure<GroupResponse>(Error.NullValue);
    }

    public async Task<OperationResult> UpdateGroupStatusAsync()
    {
        var capstone = await capstoneService.GetCapstoneByIdAsync(currentUser.CapstoneId);
        var groupId =
            (await groupMemberRepository.GetAsync(
                gm => gm.StudentId == currentUser.UserCode && gm.IsLeader,
                default))?.GroupId;
        if (groupId is null)
            return OperationResult.Failure(Error.NullValue);

        var group = await groupRepository.GetAsync(
            g => g.Id.Equals(groupId),
            true,
            g => g.Include(group => group.GroupMembers)
        );

        if (group is null)
            return OperationResult.Failure(Error.NullValue);

        if (!group.Status.Equals(GroupStatus.Pending))
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                $"Can not update group status with group id {group.Id} from {group.Status.ToString()}"));

        try
        {
            await uow.BeginTransactionAsync();
            if (group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count <
                capstone.Value.MinMember ||
                group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count >
                capstone.Value.MaxMember)
                return OperationResult.Failure(new Error("Error.InvalidTeamSize",
                    $"Group {group.Id} have invalid team size !"));

            group.Status = GroupStatus.InProgress;
            var groupMembersPending = await groupMemberRepository.FindAsync(
                gm => gm.GroupId == groupId && gm.Status == GroupMemberStatus.UnderReview,
                null, true);
            if (groupMembersPending.Count > 0)
            {
                foreach (GroupMember groupMember in groupMembersPending)
                {
                    groupMember.Status = GroupMemberStatus.Cancelled;
                    groupMemberRepository.Update(groupMember);
                }
            }

            if (group.Status.Equals(GroupStatus.InProgress))
            {
                group.GroupCode = await GenerateGroupCode(group);
            }

            // Send notification to all groupMember
            integrationEventLogService.SendEvent(new GroupStatusUpdatedEvent
            {
                GroupCode = group.GroupCode,
                StudentCodes = group.GroupMembers
                    .Where(x => x.Status == GroupMemberStatus.Accepted)
                    .Select(x => x.StudentId).ToList(),
                GroupId = group.Id
            });

            await uow.CommitAsync();
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update status failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed", "can not update group status"));
        }
    }

    private async Task<string> GenerateGroupCode(Group group)
    {
        var nextGroupNumber = await CountInProgressGroups(group.CampusId, group.SemesterId, group.CapstoneId) + 1;

        var groupMemberCode = nextGroupNumber.ToString($"D{Math.Max(3, nextGroupNumber.ToString().Length)}");

        return $"G{group.SemesterId}{group.MajorId}{groupMemberCode}";
    }

    private async Task<string> GenerateGroupCode(int currentNumberOfGroup, Group group)
    {
        var nextGroupNumber = currentNumberOfGroup + 1;

        var groupMemberCode = nextGroupNumber.ToString($"D{Math.Max(3, nextGroupNumber.ToString().Length)}");

        return $"G{group.SemesterId}{group.MajorId}{groupMemberCode}";
    }

    private async Task<int> CountInProgressGroups(string campusId, string semesterId, string capstoneId)
    {
        return await groupRepository.CountAsync(
            g => g.Status == GroupStatus.InProgress &&
                 g.CampusId == campusId &&
                 g.SemesterId == semesterId &&
                 g.CapstoneId == capstoneId);
    }

    public async Task<OperationResult<Guid>> CreateTopicRequestAsync(TopicRequest_Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            var timeConfig = await timeConfigurationService.GetCurrentTimeConfiguration(currentUser.CampusId);

            if (timeConfig.IsFailure)
                return OperationResult.Failure<Guid>(timeConfig.Error);

            if (timeConfig.Value.IsActived &&
                (timeConfig.Value.RegistTopicDate > DateTime.Now ||
                 timeConfig.Value.RegistTopicExpiredDate < DateTime.Now))
                return OperationResult.Failure<Guid>(new Error("TopicRequest.Error",
                    "You need request topic in right time."));

            var groupMember = await groupMemberRepository
                .GetAsync(
                    gm => gm.GroupId.Equals(request.GroupId) &&
                          gm.StudentId == currentUser.UserCode &&
                          gm.IsLeader,
                    gm => gm.Include(gm => gm.Group)
                        .ThenInclude(g => g.TopicRequests)
                        .Include(gm => gm.Student),
                    default,
                    cancellationToken
                );
            // check if group member is null
            if (groupMember is null)
                return OperationResult.Failure<Guid>(Error.NullValue);

            // check if the group is from current semester
            if (groupMember.Group.IsDeleted)
                return OperationResult.Failure<Guid>(new Error("Error.InvalidGroup",
                    $"The group with Id {request.GroupId} is not in current semester"));

            // check if group status is different from InProgress
            if (!groupMember.Group.Status.Equals(GroupStatus.InProgress))
                return OperationResult.Failure<Guid>(new Error("Error.GroupInEligible",
                    $"Group with id {groupMember.GroupId} is not {GroupStatus.InProgress.ToString()} status"));

            // check if topic code of group is not null
            if (groupMember.Group.TopicId != null)
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    "Can not create topic request for group already have topic code!"));

            if (groupMember.Group.TopicRequests.Any(tr => !tr.Status.Equals(TopicRequestStatus.Rejected)))
                return OperationResult.Failure<Guid>(new Error("Error.CreateTopicRequestFailed",
                    $"Can not create topic request while this group already have topic request is {TopicRequestStatus.UnderReview.ToString()} or {TopicRequestStatus.Accepted}"));

            var topicResult = await topicService.GetTopicEntityById(request.TopicId, false, cancellationToken);

            // check if topic is not null
            if (topicResult.IsFailure)
                return OperationResult.Failure<Guid>(Error.NullValue);

            var topic = topicResult.Value;

            // check if topic's campus is the same as group's campus or if topic's capstone is different from group's capstone
            if (topic.CampusId != groupMember.Student.CampusId ||
                topic.CapstoneId != groupMember.Group.CapstoneId)
                return OperationResult.Failure<Guid>(new Error("Error.CreateTopicRequestFailed",
                    "Error.CreateTopicRequestFailed"));

            // check if topic is Passed and is not assigned to any group
            if (!topic.Status.Equals(TopicStatus.Approved) ||
                topic.IsAssignedToGroup)
                return OperationResult.Failure<Guid>(new Error("Error.CreateTopicRequestFailed",
                    "Error.CreateTopicRequestFailed"));

            // check if group was sent request to this topic before and rejected
            if (groupMember.Group.TopicRequests.Any(tr => tr.TopicId.Equals(topic.Id)))
                return OperationResult.Failure<Guid>(new Error("Error.CantSentTopicRequest",
                    $"Can not sent topic request with topic id {topic.Id} because its was sent before"));

            await uow.BeginTransactionAsync(cancellationToken);

            var topicRequest = new TopicRequest
            {
                Id = Guid.NewGuid(),
                SupervisorId = topic.MainSupervisorId,
                GroupId = groupMember.GroupId,
                TopicId = topic.Id
            };

            topicRequestRepository.Insert(topicRequest);
            await uow.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new TopicRequestCreatedEvent
            {
                GroupId = request.GroupId,
                GroupCode = groupMember.Group.GroupCode,
                SupervisorOfTopic = topic.MainSupervisorId,
                TopicShortName = topic.Abbreviation,
                TopicId = topicRequest.TopicId
            });

            integrationEventLogService.SendEvent(new ExpirationRequestEvent
            {
                RequestId = topic.Id,
                RequestType = nameof(TopicRequest),
                ExpirationDuration =
                    TimeSpan.FromHours(systemConfigService.GetSystemConfiguration().ExpirationTopicRequestDuration)
            });

            await uow.CommitAsync(cancellationToken);

            return topicRequest.Id;
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to create topic request with error {Message}", ex.Message);

            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure<Guid>(new Error("Error.CantSentTopicRequest",
                "Fail to sent request to regist topic."));
        }
    }

    public async Task<OperationResult<Dictionary<string, List<TopicRequestResponse>>>> GetTopicRequestsAsync(
        TopicRequestParams request)
    {
        var topicRequests = (await topicRequestRepository
            .FindAsync(tr =>
                    (currentUser.Role == UserRoles.Supervisor && tr.SupervisorId == currentUser.UserCode ||
                     currentUser.Role == UserRoles.Student && tr.CreatedBy == currentUser.Email) &&
                    request.Status == null || tr.Status.Equals(request.Status) &&
                    string.IsNullOrEmpty(request.SearchTerm) ||
                    tr.Topic.Code != null && tr.Topic.Code.Contains(request.SearchTerm) ||
                    tr.Topic.EnglishName.Contains(request.SearchTerm) ||
                    (currentUser.Role == UserRoles.Student
                        ? tr.Supervisor.FullName.Contains(request.SearchTerm)
                        : tr.Group.GroupCode.Contains(request.SearchTerm)),
                tr =>
                    tr.AsSplitQuery()
                        .Include(tr => tr.Group)
                        .ThenInclude(g => g.GroupMembers.Where(gm => gm.IsLeader))
                        .ThenInclude(gm => gm.Student)
                        .Include(tr => tr.Topic)
                        .Include(tr => tr.Supervisor),
                tr =>
                    tr.OrderBy(tr => tr.Group.GPA),
                tr => new TopicRequestResponse
                {
                    TopicRequestId = tr.Id,
                    GroupId = tr.GroupId,
                    TopicCode = tr.Topic.Code!,
                    GroupCode = tr.Group.GroupCode,
                    Status = tr.Status.ToString(),
                    CreatedDate = tr.CreatedDate,
                    TopicId = tr.TopicId,
                    RequestedBy = tr.CreatedBy,
                    SupervisorId = tr.SupervisorId,
                    SupervisorFullName = tr.Supervisor.FullName,
                    TopicEnglishName = tr.Topic.EnglishName,
                    LeaderFullName = tr.Group.GroupMembers.FirstOrDefault()!.Student.FullName,
                    Gpa = tr.Group.GPA
                }
            )).GroupBy(tr => tr.TopicId);
        var groupedTopicRequests =
            topicRequests.ToDictionary(tr => tr.Select(tr => tr.TopicEnglishName).First(), tr => tr.ToList());
        return groupedTopicRequests.Count < 1
            ? OperationResult.Failure<Dictionary<string, List<TopicRequestResponse>>>(Error.NullValue)
            : OperationResult.Success(groupedTopicRequests);
    }

    public async Task<OperationResult> UpdateTopicRequestStatusAsync(UpdateTopicRequestStatusRequest request)
    {
        var topicRequest = await topicRequestRepository.GetAsync(tr => tr.Id.Equals(request.TopicRequestId) &&
                                                                       tr.SupervisorId == currentUser.UserCode,
            true,
            tr => tr.AsSingleQuery()
                .Include(tr => tr.Group)
                .ThenInclude(g => g.GroupMembers
                    .Where(x => x.Status == GroupMemberStatus.Accepted))
                .Include(tr => tr.Topic));

        // check if topic request is null
        if (topicRequest is null)
            return OperationResult.Failure(Error.NullValue);

        // check if topic request is different from UnderReview
        if (!topicRequest.Status.Equals(TopicRequestStatus.UnderReview))
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                $"Cant update topic request status while its different from {TopicRequestStatus.UnderReview.ToString()}"));
        try
        {
            await uow.BeginTransactionAsync();

            // update topic request status
            topicRequest.Status = request.Status;
            // set the rest topic request status have id same with topicRequest.TopicId to Rejected   
            if (topicRequest.Status.Equals(TopicRequestStatus.Accepted))
            {
                var topicRequests = await topicRequestRepository.FindAsync(tr =>
                    tr.SupervisorId == currentUser.UserCode &&
                    tr.TopicId.Equals(topicRequest.TopicId) &&
                    !tr.Id.Equals(request.TopicRequestId));

                foreach (TopicRequest topRequest in topicRequests)
                {
                    topRequest.Status = TopicRequestStatus.Rejected;
                    topicRequestRepository.Update(topRequest);
                }

                // assign supervisor to group
                topicRequest.Group.TopicId = topicRequest.Topic.Id;
                topicRequest.Group.SupervisorId = topicRequest.Topic.MainSupervisorId;
                topicRequest.Topic.IsAssignedToGroup = true;
                //topicRequest.Topic.GroupCode = topicRequest.Group.GroupCode;
                topicRequestRepository.Update(topicRequest);
            }

            // send noti to group leader
            integrationEventLogService.SendEvent(new TopicRequestStatusUpdatedEvent
            {
                TopicId = topicRequest.TopicId,
                TopicShortName = topicRequest.Topic.EnglishName,
                Status = request.Status.ToString(),
                SupervisorOfTopicName = currentUser.Name,
                StudentCodes = topicRequest.Group.GroupMembers.Select(x => x.StudentId).ToList()
            });

            await uow.CommitAsync();

            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update topic request status failed with message: {Message}", e.Message);
            await uow.RollbackAsync();
            return OperationResult.Failure(new Error("Error.UpdateFailed", "Update topic request status failed!!"));
        }
    }

    private static Func<IQueryable<Group>, IIncludableQueryable<Group, object>> CreateIncludeForGroupResponse()
    {
        return g => g.AsSplitQuery()
            .Include(g => g.GroupMembers)
            .ThenInclude(gm => gm.Student)
            .Include(g => g.Supervisor)
            .Include(g => g.Capstone);
    }

    private static Expression<Func<Group, GroupResponse>> CreateSelectorForGroupResponse()
    {
        return g => new GroupResponse
        {
            Id = g.Id,
            SupervisorId = g.SupervisorId ?? "undefined",
            SupervisorName = g.Supervisor == null ? "undefined" : g.Supervisor.FullName,
            Status = g.Status.ToString(),
            GroupCode = g.GroupCode,
            CampusName = g.CampusId,
            CapstoneName = g.CapstoneId,
            MajorName = g.MajorId,
            SemesterName = g.SemesterId,
            TopicCode = g.Topic != null ? g.Topic.Code : "undefined",
            AverageGPA = g.GroupMembers.Any(m => m.Status == GroupMemberStatus.Accepted)
                ? g.GroupMembers.Where(m => m.Status == GroupMemberStatus.Accepted)
                    .Select(m => m.Student.GPA)
                    .Average()
                : 0,
            GroupMemberList = g.GroupMembers
                .Where(m => m.Status == GroupMemberStatus.Accepted)
                .Select(m => new GroupMemberResponse
                {
                    Id = m.Id,
                    StudentId = m.StudentId,
                    StudentFullName = m.Student.FullName,
                    IsLeader = m.IsLeader,
                    Status = m.Status.ToString(),
                    GroupId = m.GroupId,
                    CreatedBy = m.CreatedBy,
                    CreatedDate = m.CreatedDate,
                    StudentEmail = m.Student.Email,
                    GPA = m.Student.GPA,
                }),
            CurrentNumberOfGroupPerMax =
                $"{g.GroupMembers.Count(m => m.Status == GroupMemberStatus.Accepted)}/{g.Capstone.MaxMember}",
        };
    }

    public async Task<OperationResult> ImportProjectProgressFile(ImportProjectProgressRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsValidFile(request.File))
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Invalid form file"));
        }

        var group = await groupRepository.GetAsync(
            x => x.Id == request.GroupId,
            include: x => x.AsSplitQuery()
                .Include(g => g.Capstone)
                .Include(x => x.Semester)
                .Include(g => g.GroupMembers.Where(m => m.Status == GroupMemberStatus.Accepted)),
            orderBy: null,
            cancellationToken);

        if (group == null)
            return OperationResult.Failure(Error.NullValue);

        if (group.SupervisorId != currentUser.UserCode)
            return OperationResult.Failure(new Error("ProjectProgress.Error",
                $"Supervisor {currentUser.UserCode} are not mentor this group."));

        try
        {
            await uow.BeginTransactionAsync(cancellationToken);

            using var stream = new MemoryStream();

            await request.File.CopyToAsync(stream, cancellationToken);

            using XLWorkbook wb = new XLWorkbook(stream);

            // start reading the excel file
            IXLWorksheet workSheet = wb.Worksheet(1);

            var projectProgress = new ProjectProgress
            {
                MeetingDate = workSheet.Cell(IndexStartProgressingRow, 1).GetValue<string>(),
                Slot = workSheet.Cell(IndexStartProgressingRow + 1, 1).GetValue<string>(),
            };

            int durationWeeks = group.Capstone.DurationWeeks;
            int startIndex = 2;

            // read the each projectProgressWeek of ProjectProgress
            for (int i = 1; i <= durationWeeks; i++)
            {
                var week = new ProjectProgressWeek
                {
                    TaskDescription = workSheet.Cell(IndexStartProgressingRow, startIndex++).GetValue<string>() ?? "",
                    Status = ProjectProgressWeekStatus.ToDo,
                    WeekNumber = i,
                };

                projectProgress.ProjectProgressWeeks.Add(week);
            }

            group.ProjectProgress = projectProgress;
            groupRepository.Update(group);

            await uow.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new ProjectProgressCreatedEvent
            {
                GroupId = group.Id,
                StudentCodes = group.GroupMembers.Select(x => x.StudentId).ToList(),
                Type = nameof(ProjectProgressCreatedEvent),
                EndDate = group.Semester.EndDate.EndOfDay(),
                ProjectProgressId = projectProgress.Id,
                RemindDate = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), projectProgress.MeetingDate, true),
                RemindTime = TimeSpan.FromHours(7),
                SupervisorCode = group.SupervisorId,
                SupervisorName = currentUser.Name,
            });

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to import the ProjectProgress with Error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create project progress fail."));
        }
    }

    public async Task<OperationResult> UpdateProjectProgress(UpdateProjectProgressRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var projectProgress = await projectProgressRepository.GetAsync(
                x => x.Id == request.Id,
                include: x => x.Include(x => x.Group)
                    .ThenInclude(x => x.GroupMembers.Where(x => x.Status == GroupMemberStatus.Accepted)),
                orderBy: null,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(projectProgress);

            await uow.BeginTransactionAsync(cancellationToken);

            mapper.Map(request, projectProgress);

            projectProgressRepository.Update(projectProgress);

            integrationEventLogService.SendEvent(new ProjectProgressUpdatedEvent
            {
                GroupId = projectProgress.GroupId,
                ProjectProgressId = projectProgress.Id,
                Type = nameof(ProjectProgressUpdatedEvent),
                RemindDate = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), projectProgress.MeetingDate, true),
                StudentCodes = projectProgress.Group.GroupMembers.Select(x => x.StudentId).ToList()
            });

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to update the ProjectProgress with Error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Update project progress fail."));
        }
    }

    public async Task<OperationResult<FucTaskResponse>> CreateTask(CreateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == request.ProjectProgressId,
            isEnabledTracking: true,
            include: x => x.Include(x => x.FucTasks),
            orderBy: null,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure<FucTaskResponse>(Error.NullValue);

        var result = await CheckStudentsInSameGroup(
            new List<string> { request.AssigneeId, currentUser.UserCode },
            progress.GroupId,
            cancellationToken);

        if (!result)
        {
            return OperationResult.Failure<FucTaskResponse>(new Error("ProjectProgress.Error",
                "Students are not the same group."));
        }

        try
        {
            if (progress.FucTasks.Any(x => x.KeyTask == request.KeyTask))
                return OperationResult.Failure<FucTaskResponse>(new Error("ProjectProgress.Error",
                    "The key task was already taken."));

            await uow.BeginTransactionAsync(cancellationToken);

            var newTask = new FucTask
            {
                KeyTask = request.KeyTask,
                Priority = request.Priority,
                Status = FucTaskStatus.ToDo,
                AssigneeId = request.AssigneeId,
                ReporterId = currentUser.UserCode,
                Description = request.Description,
                DueDate = request.DueDate.EndOfDay(),
                Summary = request.Summary,
            };

            progress.FucTasks.Add(newTask);

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            integrationEventLogService.SendEvent(new FucTaskCreatedEvent
            {
                FucTaskId = newTask.Id,
                ProjectProgressId = request.ProjectProgressId,
                KeyTask = newTask.KeyTask,
                ReporterName = currentUser.Name,
                ReminderType = nameof(FucTaskCreatedEvent),
                NotificationFor = newTask.AssigneeId,
                RemindTimeOnDueDate = TimeSpan.FromHours(7),
                RemindInDaysBeforeDueDate = 1
            });

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success(mapper.Map<FucTaskResponse>(newTask));
        }
        catch (Exception ex)
        {
            logger.LogError("Create Task with error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure<FucTaskResponse>(new Error("ProjectProgress.Error", "Create task fail."));
        }
    }

    public async Task<OperationResult<UpdateFucTaskResponse>> UpdateTask(UpdateTaskRequest request,
        CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == request.ProjectProgressId,
            isEnabledTracking: true,
            include: x => x.Include(x => x.FucTasks.Where(t => t.Id == request.TaskId))
                .ThenInclude(t => t.FucTaskHistories),
            orderBy: null,
            cancellationToken
        );

        if (progress is null || progress.FucTasks.Single() is null)
            return OperationResult.Failure<UpdateFucTaskResponse>(Error.NullValue);

        if (progress.FucTasks.Single().Status == FucTaskStatus.Done)
            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error",
                "This task was already done."));

        if (!await CheckStudentsInSameGroup([currentUser.UserCode], progress.GroupId, cancellationToken))
            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error",
                "You can not update this task."));

        if (currentUser.UserCode != progress.FucTasks.Single().AssigneeId &&
            currentUser.UserCode != progress.FucTasks.Single().ReporterId)
        {
            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error",
                "You can update the task which is not belong to you."));
        }

        var changes = TrackingTaskHistory.GetChangedProperties(request, progress.FucTasks.Single());

        if (changes.Count == 0)
            return OperationResult.Success(new UpdateFucTaskResponse
            {
                FucTaskId = progress.FucTasks.Single().Id,
                ProjectProgressId = progress.FucTasks.Single().ProjectProgressId
            });

        try
        {
            await uow.BeginTransactionAsync(cancellationToken);

            if (changes.ContainsKey(nameof(request.DueDate))
                && currentUser.UserCode != progress.FucTasks.Single().ReporterId)
                return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error",
                    "Only reporter of this task can change the duedate."));

            if (changes.TryGetValue(nameof(request.Status), out var status)
                && (FucTaskStatus)status.NewValue! == FucTaskStatus.Done)
                progress.FucTasks.Single().CompletionDate = DateTime.Now;

            mapper.Map(request, progress.FucTasks.Single());

            foreach (var h in changes)
            {
                progress.FucTasks.Single().FucTaskHistories.Add(new FucTaskHistory
                {
                    Content = h.Key == nameof(request.Comment)
                        ? request.Comment!
                        : $"{currentUser.UserCode} changed {h.Key} from {h.Value.OldValue} to {h.Value.NewValue}.",
                });

                if (h.Key == nameof(request.AssigneeId))
                {
                    // update reminder for other person
                    integrationEventLogService.SendEvent(new FucTaskAssigneeUpdatedEvent
                    {
                        FucTaskId = progress.FucTasks.Single().Id,
                        ProjectProgressId = progress.Id,
                        NotificationFor = request.AssigneeId!,
                        ReminderType = nameof(FucTaskAssigneeUpdatedEvent)
                    });
                }

                if (h.Key == nameof(request.DueDate))
                {
                    // update reminder for other date
                    integrationEventLogService.SendEvent(new FucTaskDueDateUpdatedEvent
                    {
                        FucTaskId = progress.FucTasks.Single().Id,
                        ProjectProgressId = progress.Id,
                        ReminderType = nameof(FucTaskDueDateUpdatedEvent),
                        DueDateChangedTime = ((DateTime)h.Value.NewValue!).EndOfDay() - (DateTime)h.Value.OldValue!
                    });
                }

                if (h.Key == nameof(request.Status) && request.Status == FucTaskStatus.Done)
                {
                    // remove reminder
                    integrationEventLogService.SendEvent(new FucTaskStatusDoneUpdatedEvent
                    {
                        FucTaskId = progress.FucTasks.Single().Id,
                        ProjectProgressId = progress.Id,
                        ReminderType = nameof(FucTaskStatusDoneUpdatedEvent),
                    });
                }
            }

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success(new UpdateFucTaskResponse
            {
                FucTaskId = progress.FucTasks.Single().Id,
                ProjectProgressId = progress.FucTasks.Single().ProjectProgressId
            });
        }
        catch (Exception ex)
        {
            logger.LogError("Update Task with error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure<UpdateFucTaskResponse>(
                new Error("ProjectProgress.Error", "Update task fail"));
        }
    }

    public async Task<OperationResult<byte[]>> ExportProgressEvaluationOfGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(x => x.Id == groupId && x.TopicId != null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        var topicResult = await topicService.GetTopicById(group.TopicId ?? Guid.Empty, cancellationToken);
        if (topicResult.IsFailure)
            return OperationResult.Failure<byte[]>(topicResult.Error);

        var result = await GetProgressEvaluationOfGroup(groupId, cancellationToken);

        if (result.IsFailure)
            return OperationResult.Failure<byte[]>(result.Error);

        // get file format to process override for export to supervisor
        try
        {
            var response = await s3Service.GetFromS3(s3BucketConfiguration.FUCTemplateBucket,
                s3BucketConfiguration.EvaluationWeeklyKey);

            return response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK
                ? throw new AmazonS3Exception("Fail to get template in S3")
                : await ProcessEvaluationProjectProgressTemplate(response, topicResult.Value, result.Value,
                    cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogWarning("Something run not expectation with error: {Message}", ex.Message);

            return OperationResult.Failure<byte[]>(new Error("ProjectProgress.Error", "Progress run fail."));
        }
    }

    private static async Task<OperationResult<byte[]>> ProcessEvaluationProjectProgressTemplate(
        GetObjectResponse response,
        TopicResponse topic,
        List<EvaluationProjectProgressResponse> evaluations,
        CancellationToken cancellationToken)
    {
        using var memoryStream = new MemoryStream();

        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0;

        using var workbook = new XLWorkbook(memoryStream);

        var workSheet = workbook.Worksheet(1);

        // fill the topic information into excel file
        workSheet.Cell(2, 2).SetValue(topic.Code);
        workSheet.Cell(3, 2).SetValue($"English: {topic.EnglishName} / Vietnamese: {topic.VietnameseName}");

        // fill the evaluation information into excel file
        int startIndex = 6;
        for (int i = 0; i < evaluations.Count; i++)
        {
            // fill the students information into excel file
            workSheet.Cell(startIndex + i, 1).SetValue(evaluations[i].StudentCode);
            workSheet.Cell(startIndex + i, 2).SetValue(evaluations[i].StudentName);
            workSheet.Cell(startIndex + i, 3).SetValue(evaluations[i].StudentRole);
            workSheet.Cell(startIndex + i, 4).SetValue(evaluations[i].AverageContributionPercentage)
                .Style.NumberFormat.Format = "0.00";

            // fill the evaluations for student in file
            foreach (var item in evaluations[i].EvaluationWeeks)
            {
                workSheet.Cell(startIndex + i, 4 + item.WeekNumber).SetValue(item.ContributionPercentage)
                    .Style.NumberFormat.Format = "0.00";
            }
        }

        using var outputStream = new MemoryStream();
        workbook.SaveAs(outputStream);
        return outputStream.ToArray(); // Return modified file as byte array
    }

    public async Task<OperationResult<List<EvaluationProjectProgressResponse>>> GetProgressEvaluationOfGroup(
        Guid groupId, CancellationToken cancellationToken)
    {
        if (!await CheckSupervisorInGroup(currentUser.UserCode, groupId, cancellationToken))
            return OperationResult.Failure<List<EvaluationProjectProgressResponse>>(new Error("ProjectProgress.Error",
                "This supervisor do not have permission."));

        IList<(string, string, bool)> studentsInGroup = await groupMemberRepository.FindAsync(
            x => x.GroupId == groupId &&
                 x.Status == GroupMemberStatus.Accepted,
            include: x => x.Include(x => x.Student),
            orderBy: null,
            selector: s => new ValueTuple<string, string, bool>(s.StudentId, s.Student.FullName, s.IsLeader),
            cancellationToken);

        if (studentsInGroup.Count == 0)
            return OperationResult.Failure<List<EvaluationProjectProgressResponse>>(Error.NullValue);

        var query = from eva in weeklyEvaluationRepository.GetQueryable()
            where studentsInGroup.Select(x => x.Item1).ToList().Contains(eva.StudentId)
            group eva by eva.StudentId
            into g
            select new
            {
                StudentId = g.Key,
                Evaluations = g.Select(e => new EvaluationWeekResponse
                {
                    WeekNumber = e.ProjectProgressWeek.WeekNumber,
                    ContributionPercentage = e.ContributionPercentage,
                    Comments = e.Comments,
                    MeetingContent = e.ProjectProgressWeek.MeetingContent ?? "",
                    Summary = e.ProjectProgressWeek.ProgressWeekSummary,
                    Status = e.Status.ToString()
                }).OrderBy(x => x.WeekNumber).ToList()
            };

        var weeklyEvaluationsForGroup = await query.ToListAsync(cancellationToken);

        if (weeklyEvaluationsForGroup.Count == 0)
            return OperationResult.Failure<List<EvaluationProjectProgressResponse>>(Error.NullValue);

        var result = from student in studentsInGroup
            join evaluation in weeklyEvaluationsForGroup
                on student.Item1 equals evaluation.StudentId into evalGroup
            from eval in evalGroup.DefaultIfEmpty()
            let evaluations = eval?.Evaluations ?? new List<EvaluationWeekResponse>()
            let evaluationsCount = evaluations.Count
            select new EvaluationProjectProgressResponse
            {
                StudentCode = student.Item1,
                StudentName = student.Item2,
                StudentRole = student.Item3 ? "leader" : "member",
                EvaluationWeeks = evaluations,
                AverageContributionPercentage = evaluationsCount == 0
                    ? 0
                    : evaluations.Sum(x => x.ContributionPercentage) / evaluationsCount
            };

        return result.ToList();
    }

    public async Task<OperationResult> CreateWeeklyEvaluations(CreateWeeklyEvaluationRequest request,
        CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            predicate: x => x.Id == request.ProjectProgressId,
            isEnabledTracking: true,
            include: x => x.Include(x => x.ProjectProgressWeeks
                .Where(w => w.Id == request.ProjectProgressWeekId)),
            orderBy: null,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure(Error.NullValue);

        if (progress.ProjectProgressWeeks.Count == 0 ||
            progress.ProjectProgressWeeks.Single().Status == ProjectProgressWeekStatus.Done)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Week can not evaluation."));
        }

        if (!await CheckSupervisorAndStudentAreSameGroup(
                request.EvaluationRequestForStudents.Select(x => x.StudentId).ToList(),
                currentUser.UserCode,
                progress.GroupId,
                checkAllStudents: true,
                cancellationToken))
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error",
                "Supervisor need to evaluation your group."));
        }

        try
        {
            await uow.BeginTransactionAsync(cancellationToken);

            progress.ProjectProgressWeeks.Single().Status = ProjectProgressWeekStatus.Done;

            request.EvaluationRequestForStudents.ForEach(e =>
            {
                progress.ProjectProgressWeeks.Single().WeeklyEvaluations.Add(new WeeklyEvaluation
                {
                    Comments = e.Comments,
                    ContributionPercentage = e.ContributionPercentage,
                    Status = e.Status,
                    StudentId = e.StudentId,
                });
            });

            projectProgressRepository.Update(progress);

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Create evaluation fail with error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error",
                "Evaluation weekly for this student fail."));
        }
    }

    public async Task<OperationResult> SummaryProjectProgressWeek(SummaryProjectProgressWeekRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var progress = await projectProgressRepository.GetAsync(
                x => x.Id == request.ProjectProgressId,
                isEnabledTracking: true,
                include: x => x.Include(w => w.ProjectProgressWeeks
                    .Where(w => w.Id == request.ProjectProgressWeekId)),
                orderBy: null,
                cancellationToken);

            if (progress == null || progress.ProjectProgressWeeks.Single() == null)
                return OperationResult.Failure(Error.NullValue);

            if (!await CheckStudentIsLeader(currentUser.UserCode, progress.GroupId, cancellationToken))
                return OperationResult.Failure(new Error("ProjectProgres.Error", "You can not summary this week."));

            mapper.Map(request, progress.ProjectProgressWeeks.Single());

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Update summary for week {WeekId} fail with error {Message}", request.ProjectProgressWeekId,
                ex.Message);

            return OperationResult.Failure(new Error("ProjectProgres.Error", "Fail to summary this week."));
        }
    }

    public async Task<OperationResult<List<FucTaskResponse>>> GetTasks(Guid projectProgressId,
        CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == projectProgressId,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure<List<FucTaskResponse>>(Error.NullValue);

        if (!await CheckTheUserIsValid(progress.GroupId, cancellationToken))
            return OperationResult.Failure<List<FucTaskResponse>>(new Error("ProjectProgress.Error",
                "The user can not view tasks"));

        var tasks = await fucTaskRepository.FindAsync(
            x => x.ProjectProgressId == projectProgressId,
            include: x => x.Include(x => x.Assignee)
                .Include(x => x.Reporter),
            orderBy: x => x.OrderByDescending(x => x.CreatedDate),
            cancellationToken);

        return tasks.Select(t => new FucTaskResponse
        {
            Id = t.Id,
            KeyTask = t.KeyTask,
            Description = t.Description,
            AssigneeId = t.AssigneeId,
            ReporterId = t.ReporterId,
            DueDate = t.DueDate,
            Priority = t.Priority,
            Status = t.Status,
            Summary = t.Summary,
            CreatedDate = t.CreatedDate,
            AssigneeName = t.Assignee.FullName,
            ReporterName = t.Reporter.FullName,
            CompletionDate = t.CompletionDate,
        }).ToList();
    }

    public async Task<OperationResult<FucTaskDetailResponse>> GetTasksDetail(Guid taskId,
        CancellationToken cancellationToken)
    {
        var task = await fucTaskRepository.GetAsync(
            x => x.Id == taskId,
            include: x => x.AsSingleQuery()
                .Include(x => x.Assignee)
                .Include(x => x.Reporter)
                .Include(x => x.FucTaskHistories),
            orderBy: x => x.OrderByDescending(x => x.CreatedDate),
            cancellationToken);

        return task == null
            ? OperationResult.Failure<FucTaskDetailResponse>(Error.NullValue)
            : new FucTaskDetailResponse
            {
                Id = taskId,
                KeyTask = task.KeyTask,
                Description = task.Description,
                Summary = task.Summary,
                AssigneeId = task.AssigneeId,
                AssigneeName = task.Assignee.FullName,
                ReporterId = task.ReporterId,
                ReporterName = task.Reporter.FullName,
                Status = task.Status,
                Priority = task.Priority,
                DueDate = task.DueDate,
                CompletionDate = task.CompletionDate,
                ProjectProgressId = task.ProjectProgressId,
                FucTaskHistories = task.FucTaskHistories.Select(h => new FucTaskHistoryDto
                {
                    Id = h.Id,
                    Content = h.Content,
                    TaskId = h.TaskId,
                    CreatedDate = h.CreatedDate,
                }).ToList(),
                CreatedDate = task.CreatedDate,
                LastUpdatedDate = task.UpdatedDate
            };
    }

    public async Task<OperationResult<DashBoardFucTasksOfGroup>> DashBoardTaskOfGroup(Guid projectProgressId,
        CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == projectProgressId,
            include: x => x.Include(x => x.FucTasks),
            orderBy: null,
            cancellationToken);

        if (progress == null || progress.FucTasks.Count == 0)
            return OperationResult.Failure<DashBoardFucTasksOfGroup>(Error.NullValue);

        if (!await CheckSupervisorInGroup(currentUser.UserCode, progress.GroupId, cancellationToken))
            return OperationResult.Failure<DashBoardFucTasksOfGroup>(new Error("ProjectProgress.Error",
                "This supervisor do not have permission."));

        var fucTasks = progress.FucTasks;

        var studentTasks = fucTasks.GroupBy(x => x.AssigneeId);

        return new DashBoardFucTasksOfGroup
        {
            DashBoardFucTask = GetDashBoardFucTasksDetail(fucTasks),
            DashBoardFucTasksStudents = studentTasks.Select(x => new DashBoardFucTasksStudent
            {
                DashBoardFucTask = GetDashBoardFucTasksDetail(x.ToList()),
                StudentId = x.Key
            }).ToList()
        };
    }

    private static DashBoardFucTasksDetail GetDashBoardFucTasksDetail(List<FucTask> fucTasks)
    {
        return new DashBoardFucTasksDetail
        {
            TotalTasks = fucTasks.Count,
            TotalToDoTasks = fucTasks.Count(x =>
                x.Status == FucTaskStatus.ToDo && x.CompletionDate is null && x.DueDate <= DateTime.Now),
            TotalInprogressTasks = fucTasks.Count(x =>
                x.Status == FucTaskStatus.InProgress && x.CompletionDate is null && x.DueDate <= DateTime.Now),
            TotalDoneTasks =
                fucTasks.Count(x => x.Status == FucTaskStatus.Done && x.CompletionDate!.Value <= x.DueDate),
            TotalExpiredTasks =
                fucTasks.Count(x => x.Status == FucTaskStatus.Done && x.CompletionDate!.Value <= x.DueDate),
        };
    }

    public async Task<OperationResult<ProjectProgressDto>> GetProjectProgressByGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        if (!await CheckTheUserIsValid(groupId, cancellationToken))
            return OperationResult.Failure<ProjectProgressDto>(new Error("ProjectProgress.Error",
                "You can not go other group."));

        var projectProgress = await projectProgressRepository.GetAsync(
            x => x.GroupId == groupId,
            include: x => x.Include(w => w.ProjectProgressWeeks),
            orderBy: null,
            cancellationToken
        );

        return projectProgress == null
            ? OperationResult.Failure<ProjectProgressDto>(new Error("ProjectProgress.Error",
                "Project Progress does not exist."))
            : (OperationResult<ProjectProgressDto>)new ProjectProgressDto
            {
                Id = projectProgress.Id,
                MeetingDate = projectProgress.MeetingDate,
                Slot = projectProgress.Slot,
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
                        Summary = p.ProgressWeekSummary
                    }).ToList(),
            };
    }

    public async Task<OperationResult> UpdateProjectProgressWeek(UpdateProjectProgressWeekRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var progress = await projectProgressRepository.GetAsync(
                x => x.Id == request.ProjectProgressId,
                isEnabledTracking: true,
                include: x => x.Include(w => w.ProjectProgressWeeks
                    .Where(w => w.Id == request.ProjectProgressWeekId)),
                orderBy: null,
                cancellationToken);

            if (progress == null || progress.ProjectProgressWeeks.Single() == null)
                return OperationResult.Failure(Error.NullValue);

            if (!await CheckSupervisorInGroup(currentUser.UserCode, progress.GroupId, cancellationToken))
                return OperationResult.Failure(new Error("ProjectProgres.Error", "You can not update this week."));


            if (progress.ProjectProgressWeeks.Single().Status == ProjectProgressWeekStatus.Done)
                return OperationResult.Failure(new Error("ProjectProgress.Error", "This week is already evaluation."));

            mapper.Map(request, progress.ProjectProgressWeeks.Single());

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Update projectProgressWeek fail with error: {Message}", ex.Message);

            return OperationResult.Failure(new Error("ProjectProgress.Error",
                "Update ProjectProgressWeek fail."));
        }
    }

    private static bool IsValidFile(IFormFile file)
    {
        return file != null && file.Length > 0 &&
               file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<bool> CheckSupervisorAndStudentAreSameGroup(
        List<string> studentIds,
        string supervisorId,
        Guid groupId,
        bool checkAllStudents = false,
        CancellationToken cancellationToken = default)
    {
        if (studentIds is null || studentIds.Count == 0)
        {
            throw new InvalidOperationException();
        }

        var group = await groupRepository.GetAsync(
            x => x.Id == groupId &&
                 x.SupervisorId == supervisorId,
            include: x => x.Include(x => x.GroupMembers
                .Where(x => studentIds.Contains(x.StudentId)
                            && x.Status == GroupMemberStatus.Accepted)),
            orderBy: null,
            cancellationToken);

        if (group is null)
            return false;

        if (checkAllStudents)
        {
            var capstoneResult = await capstoneService.GetCapstoneByIdAsync(group.CapstoneId);

            if (capstoneResult.IsFailure)
                throw new InvalidOperationException();

            if (group.GroupMembers.Count < capstoneResult.Value.MinMember ||
                group.GroupMembers.Count > capstoneResult.Value.MaxMember)
                return false;
        }

        return group.GroupMembers.Count == studentIds.Count;
    }

    private async Task<bool> CheckSupervisorInGroup(
        string supervisorId,
        Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(x => x.Id == groupId, cancellationToken);
        if (group is null)
            return false;

        return group.SupervisorId == supervisorId ||
               group.Topic != null &&
               await coSupervisorRepository.AnyAsync(
                   x => x.SupervisorId == currentUser.UserCode &&
                        x.TopicId == group.TopicId,
                   cancellationToken);
    }

    private async Task<bool> CheckStudentsInSameGroup(List<string> studentIds, Guid groupId,
        CancellationToken cancellationToken)
    {
        studentIds = studentIds.Distinct().ToList();

        var members = await groupMemberRepository.FindAsync(
            x => studentIds.Contains(x.StudentId) &&
                 x.GroupId == groupId &&
                 x.Status == GroupMemberStatus.Accepted,
            cancellationToken);

        return members.Count == studentIds.Count;
    }

    private async Task<bool> CheckTheUserIsValid(Guid groupId, CancellationToken cancellationToken)
    {
        return currentUser.Role == UserRoles.Supervisor
            ? await CheckSupervisorInGroup(currentUser.UserCode, groupId, cancellationToken)
            : currentUser.Role == UserRoles.Student &&
              await CheckStudentsInSameGroup([currentUser.UserCode], groupId, cancellationToken);
    }

    private async Task<bool> CheckStudentIsLeader(string studentId, Guid groupId, CancellationToken cancellationToken)
    {
        var leader = await groupMemberRepository.GetAsync(
            x => x.StudentId == studentId &&
                 x.GroupId == groupId &&
                 x.Status == GroupMemberStatus.Accepted &&
                 x.IsLeader,
            cancellationToken);

        return leader != null;
    }

    public async Task<OperationResult<GroupResponse>> GetGroupInformationByGroupSelfId()
    {
        // get group information

        var group = await groupMemberRepository.GetAsync(gm =>
                gm.StudentId.Equals(currentUser.UserCode) &&
                gm.Status.Equals(GroupMemberStatus.Accepted) ||
                gm.Status.Equals(GroupMemberStatus.UnderReview),
            gm => new GroupResponse
            {
                Id = gm.GroupId,
                SupervisorId = gm.Group.SupervisorId ?? "undefined",
                SupervisorName = gm.Group.Supervisor == null ? "undefined" : gm.Group.Supervisor.FullName,
                Status = gm.Group.Status.ToString(),
                CampusName = gm.Group.CampusId,
                CapstoneName = gm.Group.CapstoneId,
                GroupCode = gm.Group.GroupCode,
                TopicCode = gm.Group.Topic.Code,
                MajorName = gm.Group.MajorId,
                SemesterName = gm.Group.SemesterId,
                AverageGPA = gm.Group.GroupMembers.Any(gm => gm.Status == GroupMemberStatus.Accepted)
                    ? gm.Group.GroupMembers.Where(m => m.Status == GroupMemberStatus.Accepted)
                        .Select(m => m.Student.GPA)
                        .Average()
                    : 0
            },
            gm => gm.AsSplitQuery()
                .Include(gm => gm.Student)
                .Include(gm => gm.Group)
                .ThenInclude(g => g.Topic)
                .Include(g => g.Group.Supervisor)
                .Include(gm => gm.Group.GroupMembers.Where(gm => gm.Status == GroupMemberStatus.Accepted))
                .ThenInclude(gm => gm.Student));

        if (group is null)
        {
            logger.LogError("group information is null !");
            return OperationResult.Failure<GroupResponse>(Error.NullValue);
        }

        group.GroupMemberList = await groupMemberRepository.FindAsync(gm =>
                gm.Status == GroupMemberStatus.Accepted && gm.GroupId == group.Id,
            include: gm => gm.Include(gm => gm.Student),
            orderBy: gm => gm.OrderBy(gm => gm.CreatedDate),
            selector: gm => new GroupMemberResponse()
            {
                Id = gm.Id,
                Status = gm.Status.ToString(),
                StudentEmail = gm.Student.Email,
                StudentId = gm.StudentId,
                IsLeader = gm.IsLeader,
                StudentFullName = gm.Student.FullName,
                GroupId = gm.GroupId,
                CreatedBy = gm.CreatedBy,
                CreatedDate = gm.CreatedDate,
                GPA = gm.Student.GPA
            });
        // get topic's group information
        group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        return group;
    }

    public async Task<OperationResult<IList<GroupManageBySupervisorResponse>>> GetGroupsWhichMentorBySupervisor(
        CancellationToken cancellationToken)
    {
        string supervisorId = currentUser.UserCode;

        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<IList<GroupManageBySupervisorResponse>>(currentSemester.Error);

        var groups = await groupRepository.FindAsync(
            x => x.SupervisorId == supervisorId &&
                 x.Status == GroupStatus.InProgress &&
                 x.SemesterId == currentSemester.Value.Id,
            include: x => x.Include(x => x.Topic),
            orderBy: x => x.OrderBy(x => x.GroupCode),
            selector: x => new GroupManageBySupervisorResponse
            {
                GroupId = x.Id,
                GroupCode = x.GroupCode,
                EnglishName = x.Topic!.EnglishName,
                TopicCode = x.Topic.Code,
                SemesterCode = x.SemesterId
            },
            cancellationToken);

        if (groups == null || groups.Count == 0)
            ArgumentNullException.ThrowIfNull(groups);

        return OperationResult.Success(groups);
    }

    public async Task<OperationResult<IList<GroupManageBySupervisorResponse>>> GetGroupsWhichMentorByCoSupervisor(
        CancellationToken cancellationToken)
    {
        string coSupervisorId = currentUser.UserCode;

        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<IList<GroupManageBySupervisorResponse>>(currentSemester.Error);

        var groups = await groupRepository.FindAsync(
            x => x.Topic != null &&
                 x.Topic.CoSupervisors.Any(x => x.SupervisorId == coSupervisorId) &&
                 x.Status == GroupStatus.InProgress &&
                 x.SemesterId == currentSemester.Value.Id,
            include: x => x.AsSplitQuery()
                .Include(x => x.Topic)
                .ThenInclude(x => x.CoSupervisors),
            orderBy: x => x.OrderBy(x => x.GroupCode),
            selector: x => new GroupManageBySupervisorResponse
            {
                GroupId = x.Id,
                GroupCode = x.GroupCode,
                EnglishName = x.Topic!.EnglishName,
                TopicCode = x.Topic.Code,
                SemesterCode = x.SemesterId
            },
            cancellationToken);

        if (groups == null || groups.Count == 0)
            ArgumentNullException.ThrowIfNull(groups);

        return OperationResult.Success(groups);
    }

    public async Task<OperationResult> UploadGroupDocumentForGroup(UploadGroupDocumentRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!await CheckStudentIsLeader(currentUser.UserCode, request.GroupId, cancellationToken))
                return OperationResult.Failure(new Error("Group.Error", "Only leaeder can upload group document"));

            var group = await groupRepository.GetAsync(
                x => x.Id == request.GroupId &&
                     x.Status == GroupStatus.InProgress,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(group);

            var key = $"{group.CampusId}/{group.SemesterId}/{group.MajorId}/{group.CapstoneId}/{group.GroupCode}";

            return await documentsService.CreateGroupDocument(request.File, key, cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to upload group document with error {Message}.", ex.Message);

            return OperationResult.Failure(new Error("Group.Error", "Fail to upload group document."));
        }
    }

    public async Task<OperationResult<string>> PresentGroupDocumentFileOfGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(
            x => x.Id == groupId,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        var key = $"{group.CampusId}/{group.SemesterId}/{group.MajorId}/{group.CapstoneId}/{group.GroupCode}";

        return await documentsService.PresentGroupDocumentFilePresignedUrl(key);
    }

    public async Task<OperationResult<Guid>> UpdateGroupDecisionBySupervisorIdAsync(
        UpdateGroupDecisionStatusBySupervisorRequest request)
    {
        try
        {
            var group = await groupRepository.GetAsync(
                g => g.Id == request.GroupId && g.SupervisorId == currentUser.UserCode,
                true,
                g =>
                    g.Include(g => g.Supervisor)
                        .Include(g => g.Capstone)
                        .Include(g => g.GroupMembers.Where(gm => gm.Status == GroupMemberStatus.Accepted))
                        .Include(g => g.DefendCapstoneProjectDecision)
                        .Include(g => g.ReviewCalendars.Where(rc => rc.Status == ReviewCalendarStatus.Done)));

            if (!IsGroupValidForUpdateDecisionStatus(group) &&
                group.DefendCapstoneProjectDecision != null)
                return OperationResult.Failure<Guid>(new Error("Error.UpdateFailed",
                    "can not update group decision status because group is not valid"));

            if (group!.SupervisorId != currentUser.UserCode)
                return OperationResult.Failure<Guid>(new Error("Error.UpdateFailed", "Can not update group decision"));

            // check if group is re defend capstone project
            if (group.IsReDefendCapstoneProject)
            {
                return OperationResult.Failure<Guid>(new Error("Error.UpdateFailed", "Can not update group decision"));
            }

            // get ineligible student list 
            var studentTasks = request.DisagreedToDefenseStudentIds.Select(async x =>
            {
                var student = await studentRepository.GetAsync(s => s.Id == x, true);

                return student ?? throw new ArgumentNullException();
            });

            var students = await Task.WhenAll(studentTasks);

            var numOfInEligibleStudent = group.GroupMembers.Count(gm => students.Any(s => s.Id == gm.StudentId));

            if (numOfInEligibleStudent >= group.GroupMembers.Count &&
                request.DecisionStatus != DecisionStatus.Disagree_to_defense)
                throw new ArgumentException(
                    $"Can not update group decision status while all of student are disagree to defense but decision status is not {DecisionStatus.Disagree_to_defense.ToString()}!!");
            await uow.BeginTransactionAsync();
            foreach (Student student in students)
            {
                student.Status = StudentStatus.InCompleted;
                studentRepository.Update(student);
            }

            if (numOfInEligibleStudent >= group.GroupMembers.Count)
            {
                group.Status = GroupStatus.InCompleted;
                groupRepository.Update(group);
            }
            else if ((float)numOfInEligibleStudent / group.GroupMembers.Count > 0.5)
            {
                throw new ArgumentException(
                    $"Can not update group decision status while more than 40% of student are disagree to defense !!");
            }

            var defendCapstoneProjectDecision = new DefendCapstoneProjectDecision
            {
                Id = Guid.NewGuid(),
                SupervisorId = group.SupervisorId,
                GroupId = group.Id,
                Comment = request.Comment,
                Decision = request.DecisionStatus
            };
            defendCapstoneDecisionRepository.Insert(defendCapstoneProjectDecision);

            await uow.CommitAsync();

            // send noti

            integrationEventLogService.SendEvent(new GroupDecisionUpdatedEvent
            {
                GroupId = group.Id,
                GroupCode = group.GroupCode,
                Decision = defendCapstoneProjectDecision.Decision.ToString(),
                MemberCode = group.GroupMembers.Select(x => x.StudentId).ToList()
            });
            return OperationResult.Success(defendCapstoneProjectDecision.Id);
        }
        catch (Exception e)
        {
            logger.LogError("Update group decision status failed with error {Message}.", e.Message);
            return OperationResult.Failure<Guid>(new Error("Error.UpdateFailed",
                "Update group decision status failed"));
        }
    }

    public async Task<OperationResult> UpdateGroupDecisionByPresidentIdAsync(Guid groupId, Guid calendarId,
        bool isReDefendCapstoneProject)
    {
        try
        {
            var group = await groupRepository.GetAsync(g => g.Id == groupId,
                true,
                g =>
                    g.Include(g => g.Supervisor)
                        .Include(g => g.Capstone)
                        .Include(g => g.DefendCapstoneProjectDecision)
                        .Include(g => g.ReviewCalendars.Where(rc => rc.Status == ReviewCalendarStatus.Done)));
            var president = await defendCapstoneProjectCouncilMemberRepository.GetAsync(
                cm => cm.SupervisorId == currentUser.UserCode && cm.IsPresident,
                true,
                cm => cm.Include(cm => cm.DefendCapstoneProjectInformationCalendar),
                default);

            if (president == null)
                return OperationResult.Failure(new Error("Error.UpdateFailed", "Invalid president"));

            if (president.DefendCapstoneProjectInformationCalendar.DefenseDate.Date != DateTime.Now.Date)
                return OperationResult.Failure(new Error("Error.Updated",
                    "Can not update group decision on invalid date time"));

            if (!IsGroupValidForUpdateDecisionStatus(group))
                return OperationResult.Failure(new Error("Error.UpdateFailed",
                    "can not update group decision status because group is not valid"));

            if (president.DefendCapstoneProjectInformationCalendar.TopicId != group!.TopicId)
                return OperationResult.Failure(new Error("Error.UpdateFailed", "Can not update group decision"));

            if (group.DefendCapstoneProjectDecision is null)
                return OperationResult.Failure(new Error("Error.UpdateFailed",
                    "Can not update group decision while group decision is null"));
            switch (isReDefendCapstoneProject)
            {
                case false:
                    group!.IsReDefendCapstoneProject = false;
                    group.Status = GroupStatus.Completed;
                    break;
                case true when group.DefendCapstoneProjectDecision.Decision == DecisionStatus.Agree_to_defense:
                    group.DefendCapstoneProjectDecision.Decision = DecisionStatus.Revised_for_the_second_defense;
                    group.IsReDefendCapstoneProject = true;
                    break;
                default:
                    break;
            }

            president.DefendCapstoneProjectInformationCalendar.Status = DefendCapstoneProjectCalendarStatus.Done;
            defendCapstoneProjectCouncilMemberRepository.Update(president);
            groupRepository.Update(group);
            await uow.SaveChangesAsync();
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update group decision status failed with error {Message}.", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed", "Update group decision status failed"));
        }
    }

    public async Task<OperationResult<GroupDecisionResponse>> GetGroupDecisionByGroupIdAsync(Guid groupId)
    {
        var groupDecision = await defendCapstoneDecisionRepository.GetAsync(dc => dc.GroupId == groupId,
            include: dc => dc
                .Include(dc => dc.Group).ThenInclude(g => g.GroupMembers)
                .Include(dc => dc.Group.Topic)
                .Include(dc => dc.Group.Supervisor)
                .Include(dc => dc.Supervisor));

        if (groupDecision is null)
        {
            return OperationResult.Failure<GroupDecisionResponse>(Error.NullValue);
        }

        if (currentUser.Role == UserRoles.Supervisor && groupDecision.SupervisorId != currentUser.UserCode)
            return OperationResult.Failure<GroupDecisionResponse>(new Error("GetFailed",
                "Can not get group decision while you are not mentor of this group"));
        if (currentUser.Role == UserRoles.Student &&
            groupDecision.Group.GroupMembers.Any(gm => gm.StudentId != currentUser.UserCode))
            return OperationResult.Failure<GroupDecisionResponse>(new Error("GetFailed",
                "Can not get group decision while you are not member of this group"));
        return new GroupDecisionResponse
        {
            GroupId = groupDecision.GroupId,
            Comment = groupDecision.Comment,
            Decision = groupDecision.Decision.ToString(),
            GroupCode = groupDecision.Group.GroupCode,
            TopicId = (Guid)groupDecision.Group.TopicId!,
            TopicCode = groupDecision.Group.Topic.Code,
            SupervisorName = groupDecision.Group.Supervisor.FullName
        };
    }


    private static bool IsGroupValidForUpdateDecisionStatus(Group? group)
    {
        return group != null &&
               group.Status == GroupStatus.InProgress &&
               group.ReviewCalendars.Count >= group.Capstone.ReviewCount;
    }

    public async Task<OperationResult> MergeGroupForRemainStudents(CancellationToken cancellationToken)
    {
        try
        {
            var capstone = await capstoneService.GetCapstoneByIdAsync(currentUser.CapstoneId);

            if (capstone.IsFailure)
                return OperationResult.Failure(capstone.Error);

            var semester = await semesterService.GetCurrentSemesterAsync();

            if (semester.IsFailure)
                return OperationResult.Failure(semester.Error);

            var remainStudents = await studentRepository.FindAsync(
                x => x.CampusId == currentUser.CampusId &&
                     x.CapstoneId == currentUser.CapstoneId &&
                     !x.GroupMembers.Any(x => x.Status == GroupMemberStatus.Accepted),
                include: x => x.Include(x => x.GroupMembers),
                cancellationToken);

            if (remainStudents.Count == 0)
                return OperationResult.Success();

            if (remainStudents.Count < capstone.Value.MinMember)
                return OperationResult.Failure(new Error("Group.Error",
                    "The number of remain students are not enough the valid capstone number."));

            var remainStudentsAfterGroupThemWithBusinessArea = new List<Student>();
            var newGroups = new List<Group>();

            var currentNumberOfGroups =
                await CountInProgressGroups(currentUser.CampusId, semester.Value.Id, currentUser.CapstoneId);

            await uow.BeginTransactionAsync(cancellationToken);

            foreach (var g in remainStudents.GroupBy(x => x.BusinessAreaId))
            {
                remainStudentsAfterGroupThemWithBusinessArea
                    .AddRange(await GroupTheStudentsTogether(g.ToList(), newGroups, currentNumberOfGroups,
                        capstone.Value.MinMember, semester.Value));
            }

            _ = await GroupTheStudentsTogether(remainStudentsAfterGroupThemWithBusinessArea, newGroups,
                currentNumberOfGroups, capstone.Value.MinMember, semester.Value);

            groupRepository.InsertRange(newGroups);

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Merge groups failed with error {Message}.", ex.Message);

            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("Group.Error", "Merge groups failed."));
        }
    }

    private async Task<List<Student>> GroupTheStudentsTogether(List<Student> students,
        List<Group> newGroups,
        int currentNumberOfGroups,
        int MinNumberOfStudentsInGroup,
        Semester semester)
    {
        if (students.Count == 0 || students.Count < MinNumberOfStudentsInGroup)
        {
            return students;
        }

        students = students.OrderBy(x => x.GPA).ToList();

        int numberOfGroups = (int)Math.Floor((decimal)students.Count / MinNumberOfStudentsInGroup);

        for (int i = 0; i < numberOfGroups; i++)
        {
            var studentsGroup = students
                .Skip(i * MinNumberOfStudentsInGroup)
                .Take(MinNumberOfStudentsInGroup)
                .ToList();

            var leaderId = ChooseLeaderIdFromRemainStudents(studentsGroup);

            var group = new Group
            {
                CampusId = currentUser.CampusId,
                CapstoneId = currentUser.CapstoneId,
                MajorId = currentUser.MajorId,
                SemesterId = semester.Id,
                Status = GroupStatus.InProgress,
                GroupMembers = studentsGroup.Select(x => new GroupMember
                {
                    StudentId = x.Id,
                    Status = GroupMemberStatus.Accepted,
                    IsLeader = x.Id == leaderId,
                }).ToList(),
            };

            group.GroupCode = await GenerateGroupCode(newGroups.Count + currentNumberOfGroups, group);

            newGroups.Add(group);
        }

        return students.TakeLast(students.Count % MinNumberOfStudentsInGroup)
            .ToList();
    }

    private static string ChooseLeaderIdFromRemainStudents(List<Student> students)
    {
        var maxScore = students.Max(x => x.GPA);

        return students.First(x => x.GPA == maxScore).Id;
    }

    public async Task<OperationResult> AssignRemainStudentForGroup(AssignRemainStudentForGroupRequest request,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(x => x.Id == request.GroupId,
            include: x => x.Include(x => x.GroupMembers),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        if (group.CampusId != currentUser.CampusId || group.MajorId != currentUser.MajorId ||
            group.CapstoneId != currentUser.CapstoneId)
            return OperationResult.Failure(new Error("Group.Error", "Group is not on your area."));

        var capstone = await capstoneService.GetCapstoneByIdAsync(group.CapstoneId);

        if (capstone.IsFailure)
            return OperationResult.Failure(capstone.Error);

        if (group.GroupMembers.Count(x => x.Status == GroupMemberStatus.Accepted) == capstone.Value.MaxMember)
            return OperationResult.Failure(new Error("Group.Error", "Group size is max you can not add more student."));

        var student = await studentRepository.GetAsync(x => x.Id == request.StudentId,
            include: x => x.Include(x => x.GroupMembers.Where(x => x.Status == GroupMemberStatus.Accepted)),
            orderBy: null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(student);

        if (student.GroupMembers.Count > 0)
            return OperationResult.Failure(new Error("Group.Error", "The student was in the other group."));

        if (student.CampusId != currentUser.CampusId || student.MajorId != currentUser.MajorId ||
            student.CapstoneId != currentUser.CapstoneId)
            return OperationResult.Failure(new Error("Group.Error", "Student is not on your area."));

        group.GroupMembers.Add(new GroupMember
        {
            Status = GroupMemberStatus.Accepted,
            StudentId = student.Id,
        });

        groupRepository.Update(group);

        await uow.SaveChangesAsync(cancellationToken);

        return OperationResult.Success();
    }

    public async Task<OperationResult<IEnumerable<GroupDecisionResponse>>> GetGroupDecisionsByStatus(
        DecisionStatus status)
    {
        var groups = await defendCapstoneDecisionRepository.FindAsync(dc => dc.Decision == status,
            include: dc => dc
                .Include(dc => dc.Group).ThenInclude(g => g.GroupMembers)
                .Include(dc => dc.Group.Topic)
                .Include(dc => dc.Group.Supervisor)
                .Include(dc => dc.Supervisor),
            orderBy: dc => dc.OrderBy(dc => dc.CreatedDate),
            selector: dc => new GroupDecisionResponse
            {
                GroupId = dc.GroupId,
                Comment = dc.Comment,
                Decision = dc.Decision.ToString(),
                GroupCode = dc.Group.GroupCode,
                TopicId = (Guid)dc.Group.TopicId!,
                TopicCode = dc.Group.Topic.Code,
                SupervisorName = dc.Group.Supervisor.FullName
            });

        return groups.ToList();
    }

    public async Task<OperationResult<byte[]>> ExportGroupDecisionByStatus(DecisionStatus status)
    {
        var groupDecisions = await GetGroupDecisionsByStatus(status);
        if (groupDecisions.IsFailure)
            return OperationResult.Failure<byte[]>(Error.NullValue);
        // create excel
        int stt = 1;
        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Decisions");

            var rowRange = worksheet.Range("A1:E1");
            rowRange.Style.Font.FontName = "Arial";
            rowRange.Style.Font.FontSize = 12;
            rowRange.Style.Font.FontColor = XLColor.Black;
            rowRange.Style.Font.Bold = true;

            worksheet.Cell("A1").Value = "STT";
            worksheet.Cell("B1").Value = "Group Code";
            worksheet.Cell("C1").Value = "Topic Code";
            worksheet.Cell("D1").Value = "Supervisor Name";
            worksheet.Cell("E1").Value = "Decision";

            int row = 2;
            foreach (var decision in groupDecisions.Value)
            {
                worksheet.Cell($"A{row}").Value = stt++;
                worksheet.Cell($"B{row}").Value = decision.GroupCode;
                worksheet.Cell($"C{row}").Value = decision.TopicCode;
                worksheet.Cell($"D{row}").Value = decision.SupervisorName;
                worksheet.Cell($"E{row}").Value = decision.Decision;
                row++;
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
