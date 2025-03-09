using System.Linq.Expressions;
using AutoMapper;
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
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.DTOs.TopicRequestDTO;
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
    IRepository<Topic> topicRepository,
    IRepository<TopicRequest> topicRequestRepository,
    IRepository<ProjectProgress> projectProgressRepository,
    IRepository<ProjectProgressWeek> projectProgressWeekRepository,
    IRepository<FucTask> fucTaskRepository,
    IRepository<WeeklyEvaluation> weeklyEvaluationRepository,
    IRepository<Student> studentRepository,
    IRepository<Group> groupRepository,
    ICapstoneService capstoneService) : IGroupService
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

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
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

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByMajorIdAsync(string majorId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.MajorId == majorId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCapstoneIdAsync(string capstoneId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.CapstoneId == capstoneId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCampusIdAsync(string campusId)
    {
        var groups = await groupRepository.FindAsync(
            g => g.CampusId == campusId,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<GroupResponse>> GetGroupByIdAsync(Guid id)
    {
        var group = (await groupRepository.FindAsync(g => g.Id == id,
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse())).FirstOrDefault();
        return group is null
            ? OperationResult.Failure<GroupResponse>(Error.NullValue)
            : OperationResult.Success(group);
    }

    public async Task<OperationResult<GroupResponse>> GetGroupByStudentIdAsync()
    {
        // check student have been in group
        var groupMember = await groupMemberRepository.GetAsync(
            gm => gm.StudentId == currentUser.UserCode && gm.Status.Equals(GroupMemberStatus.Accepted),
            default);
        if (groupMember is null)
            return OperationResult.Failure<GroupResponse>(Error.NullValue);

        var group = await groupRepository.GetAsync(g => g.Id == groupMember.GroupId,
            true,
            CreateIncludeForGroupResponse());
        if (group is null)
        {
            return OperationResult.Failure<GroupResponse>(Error.NullValue);
        }

        var groupMembers = group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted) ||
                                                          gm.Status.Equals(GroupMemberStatus.UnderReview)).ToList();
        group.GroupMembers = groupMembers;

        var groupResponse = new GroupResponse
        {
            Id = group.Id,
            Status = group.Status.ToString(),
            GroupCode = group.GroupCode,
            CampusName = group.Campus.Name,
            CapstoneName = group.Capstone.Name,
            MajorName = group.Major.Name,
            SemesterName = group.Semester.Name,
            TopicCode = group.TopicCode,
            GroupMemberList = group.GroupMembers.Select(gm => new GroupMemberResponse
            {
                Id = gm.Id,
                StudentId = gm.StudentId,
                StudentFullName = gm.Student.FullName,
                IsLeader = gm.IsLeader,
                Status = gm.Status.ToString(),
                GroupId = gm.GroupId,
                StudentEmail = gm.Student.Email
            })
        };
        return OperationResult.Success(groupResponse);
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
            group.Status = group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count <
                           capstone.Value.MinMember ||
                           group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count >
                           capstone.Value.MaxMember
                ? GroupStatus.Rejected
                : GroupStatus.InProgress;


            var groupCodeList = (await groupRepository.FindAsync(g => string.IsNullOrEmpty(g.GroupCode)))
                .Select(g => g.GroupCode).ToList();
            var random = new Random();

            if (group.Status.Equals(GroupStatus.InProgress))
            {
                if (groupCodeList.Count < 1)
                {
                    group.GroupCode = $"{group.SemesterId}{group.MajorId}{random.Next(1, 9999)}";
                }
                else
                {
                    do
                    {
                        group.GroupCode = $"{group.SemesterId}{group.MajorId}{random.Next(1, 9999)}";
                    } while (groupCodeList.Exists(x => x == group.GroupCode));
                }
            }

            await uow.SaveChangesAsync();
            //TODO: send notification to leader
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update status failed with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed", "can not update group status"));
        }
    }


    public async Task<OperationResult<Guid>> CreateTopicRequestAsync(TopicRequest_Request request)
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
                default
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

        var topic = await topicRepository.GetAsync(t => t.Id.Equals(request.TopicId), default);

        // check if topic is not null
        if (topic is null)
            return OperationResult.Failure<Guid>(Error.NullValue);

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

        var topicRequest = new TopicRequest
        {
            Id = Guid.NewGuid(),
            SupervisorId = topic.MainSupervisorId,
            GroupId = groupMember.GroupId,
            TopicId = topic.Id
        };
        topicRequestRepository.Insert(topicRequest);
        await uow.SaveChangesAsync();
        return topicRequest.Id;
    }

    public async Task<OperationResult<List<TopicRequestResponse>>> GetTopicRequestsAsync(
        TopicRequestParams request)
    {
        var topicRequests = await topicRequestRepository
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
                    tr.Include(tr => tr.Group)
                        .ThenInclude(g => g.GroupMembers.Where(gm => gm.IsLeader))
                        .ThenInclude(gm => gm.Student)
                        .Include(tr => tr.Topic)
                        .Include(tr => tr.Supervisor),
                tr => request.OrderBy == "_asc"
                    ? tr.OrderBy(tr => tr.CreatedDate)
                    : tr.OrderByDescending(tr => tr.CreatedDate),
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
                    LeaderFullName = tr.Group.GroupMembers.FirstOrDefault()!.Student.FullName
                }
            );


        return topicRequests.Count < 1
            ? OperationResult.Failure<List<TopicRequestResponse>>(Error.NullValue)
            : OperationResult.Success(topicRequests.ToList());
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
                .ThenInclude(gm => gm.Student)
                .Include(g => g.Major)
                .Include(g => g.Semester)
                .Include(g => g.Capstone)
                .Include(g => g.Campus);
    }

    private static Expression<Func<Group, GroupResponse>> CreateSelectorForGroupResponse()
    {
        return x => new GroupResponse
        {
            Id = x.Id,
            Status = x.Status.ToString(),
            GroupCode = x.GroupCode,
            CampusName = x.Campus.Name,
            CapstoneName = x.Capstone.Name,
            MajorName = x.Major.Name,
            SemesterName = x.Semester.Name,
            TopicCode = x.TopicCode,
            GroupMemberList = x.GroupMembers.Select(gm => new GroupMemberResponse
            {
                Id = gm.Id,
                StudentId = gm.StudentId,
                StudentFullName = gm.Student.FullName,
                IsLeader = gm.IsLeader,
                Status = gm.Status.ToString(),
                GroupId = gm.GroupId,
                StudentEmail = gm.Student.Email
            })
        };
    }

    public async Task<OperationResult<CapstoneResponse>> GetCapstoneByGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(
            x => x.Id == groupId,
            include: x => x.Include(x => x.Capstone),
            null,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        return mapper.Map<CapstoneResponse>(group.Capstone);
    }

    public async Task<bool> CheckStudentsInSameGroup(IList<string> studentIds, Guid groupId,
        CancellationToken cancellationToken)
    {
        var members = await groupMemberRepository.FindAsync(
            x => studentIds.Contains(x.StudentId) &&
                 x.GroupId == groupId &&
                 x.Status == GroupMemberStatus.Accepted,
            cancellationToken);

        return members.Count == studentIds.Count;
    }

    public async Task<OperationResult> ImportProjectProgressFile(ImportProjectProgressRequest request,
        CancellationToken cancellationToken)
    {
        if (!IsValidFile(request.File))
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Invalid form file"));
        }

        var capstoneResult = await GetCapstoneByGroup(request.GroupId, cancellationToken);

        if (capstoneResult.IsFailure)
        {
            return OperationResult.Failure(capstoneResult.Error);
        }

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
                GroupId = request.GroupId,
                SupervisorId = currentUser.UserCode,
                MeetingDate = workSheet.Cell(IndexStartProgressingRow, 1).GetValue<string>(),
            };

            int durationWeeks = capstoneResult.Value.DurationWeeks;
            int startIndex = 2;

            // read the each projectProgressWeek of ProjectProgress
            for (int i = 1; i <= durationWeeks; i++)
            {
                var week = new ProjectProgressWeek
                {
                    TaskDescription = workSheet.Cell(IndexStartProgressingRow, ++startIndex).GetValue<string>() ?? "",
                    Status = ProjectProgressWeekStatus.ToDo,
                    WeekNumber = i,
                };

                projectProgress.ProjectProgressWeeks.Add(week);
            }

            await uow.SaveChangesAsync(cancellationToken);
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

    public async Task<OperationResult> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var result = await CheckStudentsInSameGroup(new List<string> { request.AssigneeId!, currentUser.UserCode },
            request.GroupId, cancellationToken);

        if (!result)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Students are not the same group."));
        }

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

            await uow.SaveChangesAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Create Task with error: {Message}", ex.Message);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create task fail"));
        }
    }

    public async Task<OperationResult> CreateWeeklyEvaluation(CreateWeeklyEvaluationRequest request,
        CancellationToken cancellationToken)
    {
        var result = await CheckSupervisorWithStudentSameGroup([request.StudentId], currentUser.UserCode,
            request.GroupId, cancellationToken);

        if (!result)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error",
                "Supervisor need to evaluation your group."));
        }

        var week = await projectProgressWeekRepository.GetAsync(
            x => x.Id == request.ProjectProgressWeekId,
            isEnabledTracking: true,
            null, null,
            cancellationToken);

        if (week == null)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Evaluation weekly does not exist."));
        }

        if (week.Status == ProjectProgressWeekStatus.Done)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Evaluation weekly was evaluated."));
        }

        try
        {
            await uow.BeginTransactionAsync(cancellationToken);

            week.Status = ProjectProgressWeekStatus.Done;

            var evaluation = new WeeklyEvaluation
            {
                Comments = request.Comments,
                ContributionPercentage = request.ContributionPercentage,
                ProjectProgressWeekId = request.ProjectProgressWeekId,
                Status = request.Status,
                StudentId = request.StudentId,
                SupervisorId = currentUser.UserCode
            };

            weeklyEvaluationRepository.Insert(evaluation);

            await uow.SaveChangesAsync(cancellationToken);
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

    public async Task<OperationResult<ProjectProgressDto>> GetProjectProgressByGroup(Guid groupId,
        CancellationToken cancellationToken)
    {
        var projectProgress = await projectProgressRepository.GetAsync(
            x => x.GroupId == groupId,
            include: x => x.Include(w => w.ProjectProgressWeeks),
            orderBy: null,
            cancellationToken
        );

        if (projectProgress == null)
        {
            return OperationResult.Failure<ProjectProgressDto>(new Error("ProjectProgress.Error",
                "Project Progress does not exist."));
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

    public async Task<bool> CheckSupervisorWithStudentSameGroup(IList<string> studentIds,
        string supervisorId, Guid groupId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IsThisWeekBelongToProgressOfGroup(Guid weekId, Guid groupId,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<TopicOfGroupResponse>> GetGroupInformationByGroupSelfId()
    {
        // get group information
        var group = await groupMemberRepository.GetAsync(gm =>
                gm.StudentId.Equals(currentUser.UserCode) && gm.Status.Equals(GroupMemberStatus.Accepted),
            gm => new GroupResponse
            {
                Id = gm.GroupId,
                Status = gm.Group.Status.ToString(),
                CampusName = gm.Group.Campus.Name,
                CapstoneName = gm.Group.Capstone.Name,
                GroupCode = gm.Group.GroupCode,
                TopicCode = gm.Group.TopicCode,
                MajorName = gm.Group.Major.Name,
                SemesterName = gm.Group.Semester.Name,
                GroupMemberList = gm.Group.GroupMembers.Select(x => new GroupMemberResponse()
                {
                    Id = x.Id,
                    Status = x.Status.ToString(),
                    StudentEmail = x.Student.Email,
                    StudentId = x.StudentId,
                    IsLeader = x.IsLeader,
                    StudentFullName = x.Student.FullName,
                    GroupId = x.GroupId,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate
                })
            },
            gm => gm.AsSplitQuery()
                .Include(gm => gm.Student)
                .Include(gm => gm.Group)
                .Include(gm => gm.Group.Campus)
                .Include(gm => gm.Group.Capstone)
                .Include(gm => gm.Group.Semester)
                .Include(gm => gm.Group.Major)
                .Include(g => g.Group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)))
                .ThenInclude(gm => gm.Student));
        if (group is null)
        {
            logger.LogError("group information is null !");
            return OperationResult.Failure<TopicOfGroupResponse>(Error.NullValue);
        }

        // get topic's group information
        var topic = await topicRepository.GetAsync(t => t.Code.Equals(group.TopicCode),
            t => new TopicResponse
            {
                Id = t.Id.ToString(),
                Code = t.Code ?? "undefined",
                Abbreviation = t.Abbreviation,
                Description = t.Description,
                FileName = t.FileName,
                FileUrl = t.FileUrl,
                Status = t.Status.ToString(),
                CreatedDate = t.CreatedDate,
                CampusId = t.CampusId,
                CapstoneId = t.CapstoneId,
                SemesterId = t.SemesterId,
                DifficultyLevel = t.DifficultyLevel.ToString(),
                BusinessAreaName = t.BusinessArea.Name,
                EnglishName = t.EnglishName,
                VietnameseName = t.VietnameseName,
                MainSupervisorEmail = t.MainSupervisor.Email,
                MainSupervisorName = t.MainSupervisor.FullName,
                CoSupervisors = t.CoSupervisors.Select(c => new CoSupervisorDto()
                {
                    SupervisorEmail = c.Supervisor.Email,
                    SupervisorName = c.Supervisor.FullName
                }).ToList()
            },
            t => t.AsSplitQuery()
                .Include(t => t.BusinessArea)
                .Include(t => t.MainSupervisor)
                .Include(t => t.CoSupervisors)
                .ThenInclude(co => co.Supervisor));

        return new TopicOfGroupResponse(topic, group);
    }
}
