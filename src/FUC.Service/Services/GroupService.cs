using System.Linq.Expressions;
using Amazon.S3;
using Amazon.S3.Model;
using AutoMapper;
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
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicRequestDTO;
using FUC.Service.Extensions.Options;
using FUC.Service.Helpers;
using MassTransit.Initializers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;

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
    IRepository<ProjectProgressWeek> projectProgressWeekRepository,
    IRepository<ProjectProgress> projectProgressRepository,
    IRepository<FucTask> fucTaskRepository,
    IRepository<WeeklyEvaluation> weeklyEvaluationRepository,
    IRepository<Student> studentRepository,
    IRepository<Group> groupRepository,
    ITopicService topicService,
    ICapstoneService capstoneService,
    IS3Service s3Service,
    S3BucketConfiguration s3BucketConfiguration) : IGroupService
{
    private const int IndexStartProgressingRow = 2;

    public async Task<OperationResult<Guid>> CreateGroupAsync()
    {
        OperationResult<Semester> currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
            return OperationResult.Failure<Guid>(new Error("Error.SemesterIsNotGoingOn",
                "The current semester is not going on"));

        Student? leader = await studentRepository.GetAsync(
            predicate: s =>
                s.Id == currentUser.UserCode &&
                s.IsEligible &&
                !s.IsDeleted,
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
        if (leader.Status.Equals(StudentStatus.Passed) ||
            leader.GroupMembers.Count > 0 &&
            leader.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
            return OperationResult.Failure<Guid>(new Error("Error.InEligible", "Leader is ineligible to create group"));

        // Create group, group member for leader
        await uow.BeginTransactionAsync();
        var newGroup = new Group()
        {
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
        var group = (await groupRepository.FindAsync(g => g.Id == id,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse())).FirstOrDefault();
        if (group is null)
            return OperationResult.Failure<GroupResponse>(Error.NullValue);
        group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        return group;
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
                // TODO: Send Noti to leader group
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

            var groupCodeList = (await groupRepository.FindAsync(g => string.IsNullOrEmpty(g.GroupCode)))
                .Select(g => g.GroupCode).ToList();
            var random = new Random();

            if (group.Status.Equals(GroupStatus.InProgress))
            {
                var currentSemesterCode = await semesterService.GetCurrentSemesterAsync();
                var nextGroupNumber = await groupRepository.CountAsync(
                    g => g.Status == GroupStatus.InProgress &&
                         g.SemesterId == currentSemesterCode.Value.Id &&
                         g.CampusId == group.CampusId &&
                         g.CapstoneId == group.CapstoneId) + 1;

                var groupMemberCode = nextGroupNumber.ToString($"D{Math.Max(3, nextGroupNumber.ToString().Length)}");

                group.GroupCode = $"G{group.SemesterId}{group.MajorId}{groupMemberCode}";
            }

            await uow.CommitAsync();
            //TODO: send notification to leader
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update status failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed", "can not update group status"));
        }
    }

    public async Task<OperationResult<Guid>> CreateTopicRequestAsync(TopicRequest_Request request,
        CancellationToken cancellationToken)
    {
        try
        {
            // TODO: Check if the create topic request is requested in invalid date

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
            if (!string.IsNullOrEmpty(groupMember.Group.TopicCode))
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    "Can not create topic request for group already have topic code!"));

            if (groupMember.Group.TopicRequests.Any(tr => !tr.Status.Equals(TopicRequestStatus.Rejected)))
                return OperationResult.Failure<Guid>(new Error("Error.CreateTopicRequestFailed",
                    $"Can not create topic request while this group already have topic request is {TopicRequestStatus.UnderReview.ToString()} or {TopicRequestStatus.Accepted}"));

            var topicResult = await topicService.GetTopicEntityById(request.TopicId, cancellationToken);

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

            integrationEventLogService.SendEvent(new ExpirationRequestEvent 
            {
                RequestId = topic.Id,
                RequestType = nameof(TopicRequest),
                ExpirationDuration = TimeSpan.FromHours(24)
            });

            await uow.CommitAsync(cancellationToken);

            return topicRequest.Id;
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to create topic request with error {Message}", ex.Message);

            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure<Guid>(new Error("Error.CantSentTopicRequest", "Fail to sent request to regist topic."));
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
                    (currentUser.Role.Equals(UserRoles.Student)
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
            tr => tr.Include(tr => tr.Group)
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
                topicRequest.Group.TopicCode = topicRequest.Topic.Code;
                topicRequest.Group.SupervisorId = topicRequest.Topic.MainSupervisorId;
                topicRequestRepository.Update(topicRequest);
            }

            await uow.CommitAsync();
            // send noti to group leader

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
        return g =>
            g.Include(g => g.GroupMembers)
                .ThenInclude(gm => gm.Student);
    }

    private static Expression<Func<Group, GroupResponse>> CreateSelectorForGroupResponse()
    {
        return g => new GroupResponse
        {
            Id = g.Id,
            Status = g.Status.ToString(),
            GroupCode = g.GroupCode,
            CampusName = g.CampusId,
            CapstoneName = g.CapstoneId,
            MajorName = g.MajorId,
            SemesterName = g.SemesterId,
            TopicCode = g.TopicCode,
            AverageGPA = g.GroupMembers.Any(m => m.Status == GroupMemberStatus.Accepted)
                ? g.GroupMembers.Where(m => m.Status == GroupMemberStatus.Accepted)
                    .Select(m => m.Student.GPA)
                    .Average()
                : 0, // Prevent division by zero
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
                })
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
            include: x => x.Include(g => g.Capstone),
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

            // TODO: send reminderTask into process service

            // TODO: send the notification for relative people

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

    public async Task<OperationResult<FucTaskResponse>> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken)
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
            return OperationResult.Failure<FucTaskResponse>(new Error("ProjectProgress.Error", "Students are not the same group."));
        }

        try
        {
            if (progress.FucTasks.Any(x => x.KeyTask == request.KeyTask))
                return OperationResult.Failure<FucTaskResponse>(new Error("ProjectProgress.Error", "The key task was already taken."));

            await uow.BeginTransactionAsync(cancellationToken);

            var newTask = new FucTask
            {
                KeyTask = request.KeyTask,
                Priority = request.Priority,
                Status = FucTaskStatus.ToDo,
                AssigneeId = request.AssigneeId,
                ReporterId = currentUser.UserCode,
                Description = request.Description,
                DueDate = request.DueDate,
                Summary = request.Summary,
            };

            progress.FucTasks.Add(newTask);

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            // TODO: Send notification

            // TODO: send reminderTask 

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

    public async Task<OperationResult<UpdateFucTaskResponse>> UpdateTask(UpdateTaskRequest request, CancellationToken cancellationToken)
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
            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error", "This task was already done."));

        if (!await CheckStudentsInSameGroup([currentUser.UserCode], progress.GroupId, cancellationToken))
            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error", "You can not update this task."));

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

            mapper.Map(request, progress.FucTasks.Single());

            foreach (var h in changes)
            {
                progress.FucTasks.Single().FucTaskHistories.Add(new FucTaskHistory
                {
                    Content = h.Key == nameof(request.Comment)
                        ? request.Comment!
                        : $"{currentUser.UserCode} changed {h.Key} from {h.Value.OldValue} to {h.Value.NewValue}.",
                });
            }

            projectProgressRepository.Update(progress);

            await uow.SaveChangesAsync(cancellationToken);

            // TODO: Send notification

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

            return OperationResult.Failure<UpdateFucTaskResponse>(new Error("ProjectProgress.Error", "Update task fail"));
        }
    }

    public async Task<OperationResult<byte[]>> ExportProgressEvaluationOfGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(x => x.Id == groupId && !string.IsNullOrEmpty(x.TopicCode),
            cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        var topicResult = await topicService.GetTopicByCode(group.TopicCode!,
            cancellationToken);

        if (topicResult.IsFailure)
            return OperationResult.Failure<byte[]>(topicResult.Error);

        var result = await GetProgressEvaluationOfGroup(groupId, cancellationToken);

        if (result.IsFailure)
            return OperationResult.Failure<byte[]>(result.Error);

        // get file format to process overide for export to supervisor
        try
        {
            var response = await s3Service.GetFromS3(s3BucketConfiguration.FUCTemplateBucket,
                s3BucketConfiguration.EvaluationProjectProgressKey);

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
        Topic topic,
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
            workSheet.Cell(startIndex + i, 4).SetValue(evaluations[i].AverageContributionPercentage);

            // fill the evaluations for student in file
            foreach (var item in evaluations[i].EvaluationWeeks)
            {
                workSheet.Cell(startIndex + i, 4 + item.WeekNumber).SetValue(item.ContributionPercentage);
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
        var group = await groupRepository.GetAsync(
            x => x.Id == groupId &&
                 x.SupervisorId == supervisorId,
            cancellationToken);

        return group != null;
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
                Status = gm.Group.Status.ToString(),
                CampusName = gm.Group.CampusId,
                CapstoneName = gm.Group.CapstoneId,
                GroupCode = gm.Group.GroupCode,
                TopicCode = gm.Group.TopicCode,
                MajorName = gm.Group.MajorId,
                SemesterName = gm.Group.SemesterId
            },
            gm => gm.AsSplitQuery()
                .Include(gm => gm.Student)
                .Include(gm => gm.Group));

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
                CreatedDate = gm.CreatedDate
            });
        // get topic's group information
        group.TopicResponse = await topicService.GetTopicByTopicCode(group.TopicCode);
        return group;
    }

    public async Task<OperationResult<List<GroupManageBySupervisorResponse>>> GetGroupsWhichMentorBySupervisor(
        CancellationToken cancellationToken)
    {
        string supervisorId = currentUser.UserCode;

        var currentSemester = await semesterService.GetCurrentSemesterAsync();

        if (currentSemester.IsFailure)
            return OperationResult.Failure<List<GroupManageBySupervisorResponse>>(currentSemester.Error);

        var groups = await groupRepository.FindAsync(
            x => x.SupervisorId == supervisorId &&
                 x.Status == GroupStatus.InProgress &&
                 x.SemesterId == currentSemester.Value.Id,
            cancellationToken);

        if (groups == null || groups.Count == 0)
            ArgumentNullException.ThrowIfNull(groups);

        var tasks = groups.Select(async g =>
        {
            var topicResult = await topicService
                .GetTopicByCode(g.TopicCode!, cancellationToken);

            return topicResult.IsFailure
                ? throw new ArgumentNullException(topicResult.Error)
                : new GroupManageBySupervisorResponse
                {
                    GroupId = g.Id,
                    GroupCode = g.GroupCode,
                    EnglishName = topicResult.Value.EnglishName,
                    TopicCode = topicResult.Value.Code,
                    SemesterCode = g.SemesterId
                };
        });

        return (await Task.WhenAll(tasks)).OrderBy(x => x.GroupCode).ToList();
    }
}
