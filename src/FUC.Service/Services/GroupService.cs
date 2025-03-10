﻿using System.Collections.Generic;
using System.Linq.Expressions;
using Amazon.S3;
using Amazon.S3.Model;
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
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.DTOs.TopicRequestDTO;
using FUC.Service.Extensions.Options;
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
    IRepository<Topic> topicRepository,
    IRepository<TopicRequest> topicRequestRepository,
    IRepository<ProjectProgressWeek> projectProgressWeekRepository,
    IRepository<ProjectProgress> projectProgressRepository,
    IRepository<FucTask> fucTaskRepository,
    IRepository<WeeklyEvaluation> weeklyEvaluationRepository,
    IRepository<Student> studentRepository,
    IRepository<Group> groupRepository,
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
                    group.GroupCode = $"G{group.SemesterId}{group.MajorId}{random.Next(1, 9999)}";
                }
                else
                {
                    do
                    {
                        group.GroupCode = $"G{group.SemesterId}{group.MajorId}{random.Next(1, 9999)}";
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
            return OperationResult.Failure(new Error("ProjectProgress.Error", $"Supervisor {currentUser.UserCode} are not mentor this group."));

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
                    TaskDescription = workSheet.Cell(IndexStartProgressingRow, ++startIndex).GetValue<string>() ?? "",
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

    public async Task<OperationResult> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == request.ProjectProgressId,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure(Error.NullValue);

        var result = await CheckStudentsInSameGroup(
            new List<string> { request.AssigneeId!, currentUser.UserCode },
            progress.GroupId,
            cancellationToken);

        if (!result)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Students are not the same group."));
        }

        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            progress.FucTasks.Add(new FucTask
            {
                KeyTask = request.KeyTask,
                Priority = request.Priority,
                Status = FucTaskStatus.ToDo,
                AssigneeId = request.AssigneeId,
                ReporterId = currentUser.UserCode,
                Description = request.Description,
                DueDate = request.DueDate,
            });

            await uow.SaveChangesAsync(cancellationToken);

            // TODO: Send notification

            // TODO: send reminderTask 

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Create Task with error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create task fail"));
        }
    }

    public async Task<OperationResult> UpdateTask(UpdateTaskRequest request, CancellationToken cancellationToken)
    {
        var task = await fucTaskRepository.GetAsync(
            x => x.Id == request.TaskId,
            cancellationToken);

        if (task == null)
            return OperationResult.Failure(Error.NullValue);

        if (currentUser.UserCode != task.AssigneeId && currentUser.UserCode != task.ReporterId)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "You can update the task which is not belong to you."));
        }

        await uow.BeginTransactionAsync(cancellationToken);

        try
        {
            mapper.Map(request, task);

            fucTaskRepository.Update(task);

            await uow.SaveChangesAsync(cancellationToken);

            // TODO: Send notification

            // TODO: add task history

            await uow.CommitAsync(cancellationToken);

            return OperationResult.Success();
        }
        catch (Exception ex)
        {
            logger.LogError("Create Task with error: {Message}", ex.Message);
            await uow.RollbackAsync(cancellationToken);

            return OperationResult.Failure(new Error("ProjectProgress.Error", "Create task fail"));
        }
    }

    public async Task<OperationResult<byte[]>> ExportProgressEvaluationOfGroup(Guid groupId, CancellationToken cancellationToken)
    {
        var group = await groupRepository.GetAsync(x => x.Id == groupId, cancellationToken);

        ArgumentNullException.ThrowIfNull(group);

        var topic = await topicRepository.GetAsync(
            x => x.Code == group.TopicCode,
            cancellationToken);

        ArgumentNullException.ThrowIfNull(topic);

        var result = await GetProgressEvaluationOfGroup(groupId, cancellationToken);

        if (result.IsFailure)
            return OperationResult.Failure<byte[]>(result.Error);

        // get file format to process overide for export to supervisor
        try
        {
            var response = await s3Service.GetFromS3(s3BucketConfiguration.FUCTemplateBucket, s3BucketConfiguration.EvaluationProjectProgressKey);

            if (response == null || response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                throw new AmazonS3Exception("Fail to get template in S3");

            return await ProcessEvaluationProjectProgressTemplate(response, topic, result.Value, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            logger.LogWarning("Something run not expectation with error: {Message}", ex.Message);

            return OperationResult.Failure<byte[]>(new Error("ProjectProgress.Error", "Progress run fail."));
        }
    }

    private static async Task<OperationResult<byte[]>> ProcessEvaluationProjectProgressTemplate(GetObjectResponse response,
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
        return outputStream.ToArray();  // Return modified file as byte array
    }

    public async Task<OperationResult<List<EvaluationProjectProgressResponse>>> GetProgressEvaluationOfGroup(Guid groupId, CancellationToken cancellationToken)
    {
        if (await CheckSupervisorInGroup(currentUser.UserCode, groupId, cancellationToken))
            return OperationResult.Failure<List<EvaluationProjectProgressResponse>>(new Error("ProjectProgress.Error", "This supervisor do not have permission."));

        IList<(string, string, bool)> studentsInGroup = await groupMemberRepository.FindAsync(
            x => x.GroupId == groupId &&
            x.Status == GroupMemberStatus.Accepted,
            include: x => x.Include(x => x.Student),
            orderBy: null,
            selector: s => new ValueTuple<string, string, bool>(s.StudentId, s.Student.FullName, s.IsLeader),
            cancellationToken);

        if (studentsInGroup.Count == 0)
            return OperationResult.Failure<List<EvaluationProjectProgressResponse>>(Error.NullValue);

        var tasks = studentsInGroup.Select(async s =>
        {
            var weekEvaluations = await weeklyEvaluationRepository.FindAsync(
                x => x.StudentId == s.Item1,
                include: x => x.Include(x => x.ProjectProgressWeek),
                orderBy: null,
                cancellationToken);

            if (weekEvaluations == null || weekEvaluations.Count == 0)
                ArgumentNullException.ThrowIfNull(weekEvaluations);

            var evaluationWeeks = weekEvaluations.Select(e => new EvaluationWeekResponse
            {
                WeekNumber = e.ProjectProgressWeek.WeekNumber,
                ContributionPercentage = e.ContributionPercentage,
                Comments = e.Comments,
                MeetingContent = e.ProjectProgressWeek.MeetingContent ?? "",
                TaskDescription = e.ProjectProgressWeek.TaskDescription,
            }).OrderBy(x => x.WeekNumber).ToList();

            return new EvaluationProjectProgressResponse
            {
                StudentCode = s.Item1,
                StudentName = s.Item2,
                StudentRole = s.Item3 ? "leader" : "member",
                EvaluationWeeks = evaluationWeeks,
                AverageContributionPercentage = evaluationWeeks.Sum(x => x.ContributionPercentage) / evaluationWeeks.Count
            };
        });

        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<OperationResult> CreateWeeklyEvaluation(CreateWeeklyEvaluationRequest request, CancellationToken cancellationToken)
    {
        var week = await projectProgressWeekRepository.GetAsync(
            x => x.Id == request.ProjectProgressWeekId, cancellationToken);

        if (week == null)
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Week can not evaluation."));
        }

        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == week.ProjectProgressId,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure(Error.NullValue);

        if (!await CheckSupervisorAndStudentAreSameGroup([request.StudentId],
            currentUser.UserCode,
            progress.GroupId,
            cancellationToken))
        {
            return OperationResult.Failure(new Error("ProjectProgress.Error", "Supervisor need to evaluation your group."));
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

    public async Task<OperationResult<List<FucTaskResponse>>> GetTasks(Guid projectProgressId, CancellationToken cancellationToken)
    {
        var progress = await projectProgressRepository.GetAsync(
            x => x.Id == projectProgressId,
            cancellationToken);

        if (progress == null)
            return OperationResult.Failure<List<FucTaskResponse>>(Error.NullValue);

        if (!await CheckTheUserIsValid(progress.GroupId, cancellationToken))
            return OperationResult.Failure<List<FucTaskResponse>>(new Error("ProjectProgress.Error", "The user can not view tasks"));

        var tasks = await fucTaskRepository.FindAsync(
            x => x.ProjectProgressId == projectProgressId,
            cancellationToken);

        return tasks.Select(t => new FucTaskResponse
        {
            Id = t.Id,
            KeyTask = t.KeyTask,
            AssigneeId = t.AssigneeId,
            ReporterId = t.ReporterId,
            DueDate = t.DueDate,
            Priority = t.Priority,
            Status = t.Status,
            Summary = t.Summary,
            CreatedDate = t.CreatedDate,
        }).ToList();
    }

    public async Task<OperationResult<FucTaskDetailResponse>> GetTasksDetail(Guid taskId, CancellationToken cancellationToken)
    {
        var task = await fucTaskRepository.GetAsync(
            x => x.Id == taskId,
            include: x => x.AsSingleQuery()
                .Include(x => x.Assignee)
                .Include(x => x.Reporter)
                .Include(x => x.FucTaskHistories),
            orderBy: x => x.OrderByDescending(x => x.CreatedDate),
            cancellationToken);

        if (task == null)
            return OperationResult.Failure<FucTaskDetailResponse>(Error.NullValue);

        return new FucTaskDetailResponse
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
            Comment = task.Comment,
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
            return OperationResult.Failure<ProjectProgressDto>(new Error("ProjectProgress.Error", "You can not go other group."));

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

    private async Task<bool> CheckSupervisorAndStudentAreSameGroup(
        IList<string> studentIds,
        string supervisorId,
        Guid groupId,
        CancellationToken cancellationToken = default)
    {
        if (studentIds is null || studentIds.Count == 0)
        {
            throw new InvalidOperationException();
        }

        var group = await groupRepository.GetAsync(
            x => x.Id == groupId &&
            (supervisorId == null || x.SupervisorId == supervisorId),
            include: x => x.Include(x => x.GroupMembers
                .Where(x => studentIds.Contains(x.StudentId)
                    && x.Status == GroupMemberStatus.Accepted)),
            orderBy: null,
            cancellationToken);

        if (group == null ||
            group.GroupMembers.Count == 0)
        {
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

    private async Task<bool> CheckStudentsInSameGroup(IList<string> studentIds, Guid groupId,

        CancellationToken cancellationToken)
    {
        var members = await groupMemberRepository.FindAsync(
            x => studentIds.Contains(x.StudentId) &&
                 x.GroupId == groupId &&
                 x.Status == GroupMemberStatus.Accepted,
            cancellationToken);

        return members.Count == studentIds.Count;
    }

    private async Task<bool> CheckTheUserIsValid(Guid groupId, CancellationToken cancellationToken)
    {
        return currentUser.Role == UserRoles.Supervisor ?
            await CheckSupervisorInGroup(currentUser.UserCode, groupId, cancellationToken) :
                currentUser.Role == UserRoles.Student &&
                await CheckStudentsInSameGroup([currentUser.UserCode], groupId, cancellationToken);
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

    public async Task<OperationResult<List<GroupManageBySupervisorResponse>>> GetGroupsWhichMentorBySupervisor(CancellationToken cancellationToken)
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
            var topic = await topicRepository.GetAsync(
                x => x.Code == g.GroupCode,
                cancellationToken);

            ArgumentNullException.ThrowIfNull(topic);

            return new GroupManageBySupervisorResponse
            {
                GroupId = g.Id,
                GroupCode = g.GroupCode,
                EnglishName = topic.EnglishName,
                TopicCode = topic.Code,
                SemesterCode = g.SemesterId
            };
        });

        return (await Task.WhenAll(tasks)).OrderBy(x => x.GroupCode).ToList();
    }
}
