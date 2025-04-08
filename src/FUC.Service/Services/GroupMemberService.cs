using System.Linq.Expressions;
using FUC.Common.Abstractions;
using FUC.Common.Contracts;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Enums;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupMemberDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FUC.Service.Services;

public class GroupMemberService(
    IUnitOfWork<FucDbContext> uow,
    IIntegrationEventLogService integrationEventLogService,
    IGroupService groupService,
    ICurrentUser currentUser,
    ILogger<GroupMemberService> logger,
    ICapstoneService capstoneService,
    IRepository<JoinGroupRequest> joinGroupRequestRepository,
    IRepository<GroupMember> groupMemberRepository,
    IRepository<Student> studentRepository,
    ISystemConfigurationService systemConfigService) : IGroupMemberService

{
    public async Task<OperationResult<Guid>> CreateGroupMemberByLeaderAsync(CreateGroupMemberByLeaderRequest request)
    {
        try
        {
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

            // Check if Leader is eligible to send add member request 
            if (leader.GroupMembers.FirstOrDefault(gm => gm.IsLeader) is null ||
                !leader.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)) ||
                !leader.GroupMembers.Any(s => s.IsLeader))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",
                    $"User with id {currentUser.UserCode} is ineligible to add member"));


            // get number of members group's leader
            Guid groupIdOfLeader = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
            int numOfMembers = (await groupMemberRepository.FindAsync(
                gm => gm.GroupId.Equals(groupIdOfLeader) &&
                      (gm.Status.Equals(GroupMemberStatus.Accepted) ||
                       gm.Status.Equals(GroupMemberStatus.UnderReview)))).Count;
            // Check if the team size is invalid 
            if (numOfMembers + 1 > leader.Capstone.MaxMember)
                return OperationResult.Failure<Guid>(new Error("Error.MemberRequestSizeInvalid",
                    "The member request size is invalid"));


            Guid groupId = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
            // create group member for member  
            await uow.BeginTransactionAsync();
            //check if memberId value is duplicate with leaderId 
            if (request.MemberEmail.Equals(leader.Id))
                return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue",
                    $"The member id with {request.MemberEmail} was duplicate with leader id {leader.Id}"));

            // check if member is eligible to send join group request
            Student? member = await studentRepository.GetAsync(
                predicate: s => s.Email.ToLower().Equals(request.MemberEmail.ToLower()) &&
                                s.CampusId == leader.CampusId &&
                                s.CapstoneId == leader.CapstoneId &&
                                s.Status.Equals(StudentStatus.InProgress) &&
                                !s.IsDeleted,
                include: s => s.Include(s => s.GroupMembers),
                orderBy: default,
                cancellationToken: default);
            if (member is null ||
                member.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",
                    $"Member with email {request.MemberEmail} is ineligible !!"));

            if (member.GroupMembers.Any(s => s.GroupId == groupIdOfLeader &&
                                             s.Status.Equals(GroupMemberStatus.UnderReview)))
                return OperationResult.Failure<Guid>(new Error("Error.CreateGroupMemberFail",
                    $"Can not send request to member with email {member.Email} because the leader already send request to this member"));
            // create group member for member
            var newGroupMember = new GroupMember
            {
                GroupId = groupId,
                StudentId = member.Id,
                IsLeader = false
            };

            groupMemberRepository.Insert(newGroupMember);

            await uow.SaveChangesAsync();

            integrationEventLogService.SendEvent(new GroupMemberCreatedEvent
            {
                GroupId = groupId,
                MemberId = member.Id,
                GroupMemberId = newGroupMember.Id,
                LeaderId = currentUser.UserCode,
                LeaderName = currentUser.Name,
            });

            integrationEventLogService.SendEvent(new ExpirationRequestEvent
            {
                RequestId = newGroupMember.Id,
                RequestType = nameof(GroupMember),
                ExpirationDuration =
                    TimeSpan.FromHours(systemConfigService.GetSystemConfiguration().ExpirationTeamUpDuration)
            });

            await uow.CommitAsync();
            return groupId;
        }
        catch (Exception ex)
        {
            logger.LogError("Fail to create groupMember with error: {Message}", ex.Message);

            await uow.RollbackAsync();

            return OperationResult.Failure<Guid>(new Error("GroupMember.Error", "Fail to create group member."));
        }
    }

    public async Task<OperationResult> UpdateGroupMemberStatusAsync(UpdateGroupMemberRequest request)
    {
        try
        {
            // get all request of member
            // string? memberRequestId = !request.Status.Equals(GroupMemberStatus.Cancelled)
            //     ? currentUser.UserCode
            //     : request.MemberId;

            var groupMember = await groupMemberRepository.GetAsync(
                gm => gm.Id == request.Id && gm.GroupId == request.GroupId,
                isEnabledTracking: true,
                gm => gm
                    .Include(grm => grm.Group)
                    .ThenInclude(g => g.Capstone)
                    .Include(gm => gm.Student),
                cancellationToken: default);


            // check if member requests is null
            // check if the group member request is null 
            if (groupMember is null)
                return OperationResult.Failure(new Error("Error.NotFound",
                    $"The group member with id {request.Id} was not found !!"));

            // check if member have been in group before
            var memberHaveBeenInGroupBefore = await groupMemberRepository.FindAsync(gm =>
                gm.Status.Equals(GroupMemberStatus.Accepted) && gm.StudentId == groupMember.StudentId);

            if (memberHaveBeenInGroupBefore.Any() && !request.Status.Equals(GroupMemberStatus.LeftGroup))
                return OperationResult.Failure(new Error("Error.JoinedGroup",
                    $"The member with id {currentUser.UserCode} have been in group before!!!"));
            var leader =
                await groupMemberRepository.GetAsync(
                    gm => gm.IsLeader &&
                          gm.GroupId == request.GroupId &&
                          gm.StudentId == currentUser.UserCode, default);
            await uow.BeginTransactionAsync();

            // Update status for requests
            switch (request.Status)
            {
                // Accepted - Rejected - UnderReview
                case GroupMemberStatus.Accepted:
                case GroupMemberStatus.Rejected:
                    // check if the group status is not in pending status
                    if (!groupMember.Group.Status.Equals(GroupStatus.Pending))
                        return OperationResult.Failure(new Error("Error.UpdateFailed",
                            $"Can not update group member status with group status is different from {GroupStatus.Pending.ToString()}"));
                    if (!groupMember.Status.Equals(GroupMemberStatus.UnderReview))
                        return OperationResult.Failure(new Error("Error.UpdateFailed",
                            $"Can not update from {GroupMemberStatus.Accepted.ToString()} or {GroupMemberStatus.Rejected.ToString()} to {GroupMemberStatus.Accepted.ToString()} !!!"));
                    if (groupMember.StudentId != currentUser.UserCode)
                        return OperationResult.Failure(new Error("Error.UpdateFailed",
                            $"Can not update status with group member id {groupMember.Id}!!"));

                    groupMember.Status = request.Status;
                    groupMemberRepository.Update(groupMember);
                    if (request.Status.Equals(GroupMemberStatus.Accepted))
                    {
                        groupMember.Group.GPA += groupMember.Student.GPA / (groupMember.Group.GroupMembers.Count + 1);
                        var memberRequests = await groupMemberRepository.FindAsync(gm =>
                            gm.StudentId == groupMember.StudentId &&
                            gm.Id != request.Id &&
                            gm.Status.Equals(GroupMemberStatus.UnderReview), null, true);
                        if (memberRequests.Any())
                        {
                            foreach (var memberRequest in memberRequests.Where(gm => gm.Id != request.Id).ToList())
                            {
                                memberRequest.Status = GroupMemberStatus.Rejected;
                                groupMemberRepository.Update(memberRequest);
                            }
                        }
                    }

                    break;
                case GroupMemberStatus.LeftGroup:
                    if (!groupMember.Status.Equals(GroupMemberStatus.Accepted))
                        return OperationResult.Failure(new Error("Error.UpdateFailed",
                            $"Can not left group from the other status different {GroupMemberStatus.Accepted.ToString()} status"));
                    if (groupMember.IsLeader)
                    {
                        // auto change another group member status to left group with group member with status accept and to cancelled with group member with status under review and then noti to members
                        var groupMembers = await groupMemberRepository.FindAsync(
                            gm => gm.GroupId == groupMember.GroupId,
                            null,
                            true);
                        if (groupMembers.Count > 0)
                        {
                            foreach (GroupMember member in groupMembers)
                            {
                                if (member.Status == GroupMemberStatus.Accepted)
                                {
                                    member.Status = GroupMemberStatus.LeftGroup;
                                }
                                else if (member.Status == GroupMemberStatus.UnderReview)
                                {
                                    member.Status = GroupMemberStatus.Cancelled;
                                }

                                groupMemberRepository.Update(member);
                            }
                        }
                    }
                    else
                    {
                        if (leader is null)
                            throw new ArgumentException("Can not kick member while you are not leader");

                        groupMember.Status = GroupMemberStatus.LeftGroup;
                        if (groupMember.Group.Status == GroupStatus.InProgress)
                        {
                            groupMember.Student.Status = StudentStatus.InCompleted;
                        }

                        groupMemberRepository.Update(groupMember);
                    }

                    break;
                case GroupMemberStatus.Cancelled:
                    // check if the group status is not in pending status
                    if (!groupMember.Group.Status.Equals(GroupStatus.Pending))
                        return OperationResult.Failure(new Error("Error.UpdateFailed",
                            $"Can not update group member status with group status is different from {GroupStatus.Pending.ToString()}"));
                    if (leader is null)
                        throw new InvalidOperationException("Can not cancel member while you are not leader");
                    groupMember.Status = GroupMemberStatus.Cancelled;
                    groupMemberRepository.Update(groupMember);
                    break;
                default:
                    return OperationResult.Failure(new Error("Error.UpdateFailed",
                        $"Can not update status with group member id {groupMember.Id}!!"));
            }

            integrationEventLogService.SendEvent(new GroupMemberStatusUpdatedEvent
            {
                GroupMemberId = groupMember.Id,
                LeaderCode = await GetLeaderIdOfGroup(groupMember.GroupId),
                Status = request.Status.ToString(),
                MemberCode = currentUser.UserCode
            });

            await uow.CommitAsync();

            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Update group member status failed with message : {Message}", e.Message);
            return OperationResult.Failure(new Error("UpdateFailed", "Update group member status failed !!"));
        }
    }

    public async Task<OperationResult<GroupMemberRequestResponse>> GetGroupMemberRequestByMemberId()
    {
        var groupMemberRequestResponse = new GroupMemberRequestResponse();

        var groupMembers = await (from gm in groupMemberRepository.GetQueryable()
            where gm.StudentId == currentUser.UserCode
            join s in studentRepository.GetQueryable() on gm.CreatedBy equals s.Email
            orderby s.GPA, gm.CreatedDate
            select new GroupMemberResponse
            {
                Id = gm.Id,
                Status = gm.Status.ToString(),
                GroupId = gm.GroupId,
                StudentId = s.Id,
                StudentFullName = s.FullName,
                StudentEmail = s.Email,
                IsLeader = gm.IsLeader,
                CreatedDate = gm.CreatedDate,
                CreatedBy = gm.CreatedBy,
                GPA = s.GPA
            }).ToListAsync();


        groupMemberRequestResponse.GroupMemberRequested = groupMembers.Where(gm => !gm.IsLeader).ToList();

        var leaderGroup = groupMembers
            .Find(gm => gm.IsLeader);

        if (leaderGroup != null)
        {
            IList<GroupMemberResponse> groupMembersRequestOfLeader =
                await groupMemberRepository.FindAsync(
                    gm => gm.CreatedBy == leaderGroup.CreatedBy &&
                          !gm.IsLeader,
                    gm => gm.Include(gm => gm.Student),
                    x => x.OrderBy(x => x.CreatedDate),
                    x => new GroupMemberResponse()
                    {
                        Id = x.Id,
                        Status = x.Status.ToString(),
                        GroupId = x.GroupId,
                        StudentId = x.StudentId,
                        StudentFullName = x.Student.FullName,
                        StudentEmail = x.Student.Email,
                        IsLeader = x.IsLeader,
                        CreatedDate = x.CreatedDate,
                        CreatedBy = x.CreatedBy,
                        GPA = x.Student.GPA
                    });
            groupMemberRequestResponse.GroupMemberRequestSentByLeader = groupMembersRequestOfLeader.ToList();

            // get Join group requested

            groupMemberRequestResponse.JoinGroupRequested =
                (await joinGroupRequestRepository.FindAsync(
                    jr => jr.GroupId.Equals(leaderGroup.GroupId),
                    jr => jr.Include(jr => jr.Student),
                    jr => jr.OrderBy(jr => jr.CreatedDate),
                    x => new GroupMemberResponse()
                    {
                        Id = x.Id,
                        Status = x.Status.ToString(), // status of join group request
                        GroupId = x.GroupId,
                        StudentId = x.StudentId, // member id
                        StudentFullName = x.Student.FullName,
                        StudentEmail = x.Student.Email,
                        CreatedDate = x.CreatedDate,
                        CreatedBy = x.CreatedBy,
                        GPA = x.Student.GPA
                    })).ToList();
        }

        // Get all join group requests sent by the current user
        var joinGroupRequests = await joinGroupRequestRepository.FindAsync(
            jr => jr.CreatedBy == currentUser.Email,
            jr => jr.Include(jr => jr.Group)
                .ThenInclude(gr => gr.GroupMembers.Where(gm => gm.IsLeader))
                .ThenInclude(gm => gm.Student),
            jr => jr.OrderBy(jr => jr.CreatedDate)
        );

        groupMemberRequestResponse.JoinGroupRequestSentByMember = joinGroupRequests.Select(x =>
        {
            var leader = x.Group.GroupMembers.FirstOrDefault(); // Get the leader directly
            return new GroupMemberResponse
            {
                Id = x.Id,
                Status = x.Status.ToString(), // Status of join group request
                GroupId = x.GroupId,
                StudentId = leader!.StudentId, // Leader ID
                StudentFullName = leader!.Student.FullName,
                StudentEmail = leader!.Student.Email,
                CreatedDate = x.CreatedDate,
                CreatedBy = x.CreatedBy,
                GPA = leader?.Student.GPA ?? 0 // Handle null leader case
            };
        }).ToList();


        return OperationResult.Success(groupMemberRequestResponse);
    }

    public async Task<OperationResult<Guid>> CreateJoinGroupRequestByMemberAsync(CreateJoinGroupRequestByMember request)
    {
        try
        {
            var group = await groupService.GetGroupByIdAsync(request.GroupId);
            if (group.IsFailure)
            {
                return OperationResult.Failure<Guid>(Error.NullValue);
            }

            // check if group status is In Progress
            if (!Enum.Parse<GroupStatus>(group.Value.Status).Equals(GroupStatus.Pending))
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    $"Can not send join group request to the group have status different from {GroupStatus.Pending.ToString()}"));

            var student = await studentRepository.GetAsync(
                s => s.Id == currentUser.UserCode,
                s => s.Include(s => s.JoinGroupRequests)
                    .Include(s => s.Capstone)
                    .Include(s => s.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted))),
                null);

            if (student is null)
                return OperationResult.Failure<Guid>(Error.NullValue);

            // check if student campus and capstone is different from group
            if (student.CampusId != group.Value.CampusName ||
                student.CapstoneId != group.Value.CapstoneName)
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    "Can not send join group request to group with another capstone or another campus"));

            // check the team size of group is exceed
            var maxMemberInGroup = student.Capstone.MaxMember;
            if (group.Value.GroupMemberList.Count(
                    gm => gm.Status == GroupMemberStatus.Accepted.ToString()) >= maxMemberInGroup)
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed", "The group is full of member"));

            // check if the previous join group request of current member still pending/underreview
            var previousJoinGroupRequest = student.JoinGroupRequests.MaxBy(x => x.CreatedDate);

            if (previousJoinGroupRequest is { Status: JoinGroupRequestStatus.Pending })
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    "Can not send join group request while the previous request still pending"));

            // check if student is already in group
            if (student.GroupMembers.FirstOrDefault() != null)
            {
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    $"Student with id {student.Id} is already in group before"));
            }

            // check if leader send join group request to their group self
            if (group.Value.GroupMemberList.Any(gm => gm.StudentId == student.Id && gm.IsLeader))
            {
                return OperationResult.Failure<Guid>(new Error("Error.CreateFailed",
                    "Can not send request to your group self"));
            }

            await uow.BeginTransactionAsync();

            // create join group request
            var newJoinGroupRequest = new JoinGroupRequest
            {
                Id = Guid.NewGuid(),
                StudentId = student.Id,
                GroupId = group.Value.Id
            };

            joinGroupRequestRepository.Insert(newJoinGroupRequest);

            // Send notification
            integrationEventLogService.SendEvent(new JoinGroupRequestCreatedEvent
            {
                LeaderCode = group.Value.GroupMemberList.First(x => x.IsLeader).StudentId,
                MemberCode = student.Id,
                MemberName = student.FullName,
                GroupId = group.Value.Id,
                Id = newJoinGroupRequest.Id,
            });

            // Send expiration of request
            integrationEventLogService.SendEvent(new ExpirationRequestEvent
            {
                RequestId = newJoinGroupRequest.Id,
                RequestType = nameof(JoinGroupRequest),
                ExpirationDuration =
                    TimeSpan.FromHours(systemConfigService.GetSystemConfiguration().ExpirationTeamUpDuration)
            });

            await uow.CommitAsync();

            return newJoinGroupRequest.Id;
        }
        catch (Exception e)
        {
            logger.LogError("Create Group Member Failed with error message {Message}", e.Message);
            await uow.RollbackAsync();

            return OperationResult.Failure<Guid>(new Error("Error.CreateFailed", "create group member failed"));
        }
    }


    public async Task<OperationResult<IEnumerable<GroupMember>>> GetGroupMemberByGroupId(Guid groupId)
    {
        var groupMembers = await groupMemberRepository.FindAsync(
            gm => gm.GroupId == groupId,
            null, true);
        return groupMembers.ToList();
    }

    /// <summary>
    /// Creates an expression that can be used to select a <see cref="GroupMember"/> into a <see cref="GroupMemberResponse"/>.
    /// </summary>
    /// <returns>An expression that can be used to select a <see cref="GroupMember"/> into a <see cref="GroupMemberResponse"/>.</returns>
    private static Expression<Func<GroupMember, GroupMemberResponse>> CreateSelectorGroupMember()
    {
        return x => new GroupMemberResponse
        {
            Id = x.Id,
            Status = x.Status.ToString(),
            GroupId = x.GroupId,
            StudentId = x.StudentId,
            StudentFullName = x.Student.FullName,
            StudentEmail = x.Student.Email,
            IsLeader = x.IsLeader,
            GPA = x.Student.GPA,
            CreatedDate = x.CreatedDate,
            CreatedBy = x.CreatedBy
        };
    }

    public async Task<OperationResult> UpdateJoinGroupRequestAsync(UpdateJoinGroupRequest request)
    {
        var joinGroupRequest = await joinGroupRequestRepository.GetAsync(
            jr => jr.Id == request.Id,
            true,
            include: jr => jr
                .Include(jr => jr.Student)
                .Include(jr => jr.Group)
                .ThenInclude(g => g.GroupMembers));

        if (joinGroupRequest is null)
            return OperationResult.Failure(Error.NullValue);

        var group = joinGroupRequest.Group;
        var student = joinGroupRequest.Student;

        // check join group request status is not pending
        if (IsJoinGroupRequestStatusPending(joinGroupRequest))
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                $"status of join group request with id {joinGroupRequest.Id} is not pending"));


        // check if current user is leader of this group
        var isLeader = IsCurrentUserGroupLeader(group);

        // check if leader is update join group status and group is full
        var maxMember = await capstoneService.GetMaxMemberByCapstoneId(student.CapstoneId);
        if (maxMember.IsFailure)
            return OperationResult.Failure(new Error("Error.UpdateFailed", "Can not get max member"));

        if (isLeader && IsGroupFull(group, maxMember.Value))
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                $"The group with id {group.Id} have full member"));

        try
        {
            // update join group request status
            await UpdateJoinGroupRequestStatus(joinGroupRequest, request, group, student, maxMember.Value, isLeader);
            return OperationResult.Success();
        }
        catch (Exception e)
        {
            logger.LogError("Can not update join group status with message: {Message}", e.Message);
            return OperationResult.Failure(new Error("Error.UpdateFailed", "Can not update join group request status"));
        }
    }


    private bool IsCurrentUserGroupLeader(Group group)
    {
        var leader = group.GroupMembers.FirstOrDefault(gm =>
            gm.StudentId == currentUser.UserCode &&
            gm.IsLeader);
        return leader != null;
    }


    private static bool IsJoinGroupRequestStatusPending(JoinGroupRequest joinGroupRequest)
    {
        return joinGroupRequest.Status != JoinGroupRequestStatus.Pending;
    }

    private static bool IsGroupFull(Group group, int maxMember)
    {
        return group.GroupMembers.Count(gm => gm.Status == GroupMemberStatus.Accepted) >= maxMember;
    }


    private async Task UpdateJoinGroupRequestStatus(
        JoinGroupRequest joinGroupRequest,
        UpdateJoinGroupRequest request,
        Group group,
        Student student,
        int maxMember,
        bool isLeader)
    {
        await uow.BeginTransactionAsync();

        try
        {
            switch (request.Status)
            {
                case JoinGroupRequestStatus.Approved when isLeader:
                    await ApproveJoinGroupRequest(joinGroupRequest, request, group, student, maxMember);
                    break;
                case JoinGroupRequestStatus.Rejected when isLeader:
                    RejectOrCancelJoinGroupRequest(joinGroupRequest, JoinGroupRequestStatus.Rejected);
                    break;
                case JoinGroupRequestStatus.Cancelled when !isLeader:
                    if (!IsCurrentJoinGroupRequestCorrect(joinGroupRequest))
                        throw new InvalidOperationException("Can not update join group request status");
                    RejectOrCancelJoinGroupRequest(joinGroupRequest, JoinGroupRequestStatus.Cancelled);
                    break;
                default:
                    throw new InvalidOperationException("Can not update join group request status");
            }

            integrationEventLogService.SendEvent(new JoinGroupRequestStatusUpdatedEvent
            {
                GroupId = group.Id,
                JoinGroupRequestId = joinGroupRequest.Id,
                LeaderCode = currentUser.Id,
                LeaderName = currentUser.Name,
                MemberCode = student.Id,
                Status = request.Status.ToString(),
            });

            await uow.CommitAsync();
        }
        catch (Exception)
        {
            await uow.RollbackAsync();

            throw;
        }
    }

    private bool IsCurrentJoinGroupRequestCorrect(JoinGroupRequest joinGroupRequest)
    {
        return joinGroupRequest.StudentId == currentUser.UserCode;
    }

    private async Task ApproveJoinGroupRequest(
        JoinGroupRequest joinGroupRequest,
        UpdateJoinGroupRequest request,
        Group group,
        Student student,
        int maxMember)
    {
        joinGroupRequest.Status = JoinGroupRequestStatus.Approved;

        groupMemberRepository.Insert(new GroupMember
        {
            GroupId = group.Id,
            StudentId = student.Id,
            Status = GroupMemberStatus.Accepted,
            IsLeader = false
        });

        if (IsGroupFullAfterApprove(group, maxMember))
        {
            CancelPendingInviteMemberRequests(group);
            await RejectOtherPendingJoinGroupRequests(group, request.Id);
        }
    }

    private static void RejectOrCancelJoinGroupRequest(
        JoinGroupRequest joinGroupRequest,
        JoinGroupRequestStatus status)
    {
        joinGroupRequest.Status = status;
    }

    private static bool IsGroupFullAfterApprove(
        Group group,
        int maxMember)
    {
        return group.GroupMembers.Count(gm => gm.Status == GroupMemberStatus.Accepted) + 1 >= maxMember;
    }

    private void CancelPendingInviteMemberRequests(Group group)
    {
        var membersToCancel = group.GroupMembers.Where(gm => gm.Status == GroupMemberStatus.UnderReview).ToList();
        foreach (var groupMember in membersToCancel)
        {
            groupMember.Status = GroupMemberStatus.Cancelled;
            groupMemberRepository.Update(groupMember);
        }
    }

    private async Task RejectOtherPendingJoinGroupRequests(
        Group group,
        Guid requestId)
    {
        var joinGroupRequestsToReject = await joinGroupRequestRepository.FindAsync(
            gm => gm.GroupId == group.Id &&
                  gm.Status == JoinGroupRequestStatus.Pending &&
                  gm.Id != requestId,
            null,
            true);
        if (joinGroupRequestsToReject.Count > 0)
        {
            foreach (var jg in joinGroupRequestsToReject)
            {
                jg.Status = JoinGroupRequestStatus.Rejected;
                joinGroupRequestRepository.Update(jg);
            }
        }
    }

    private async Task<string> GetLeaderIdOfGroup(Guid groupId)
    {
        var leaderId = await groupMemberRepository.GetAsync(
            x => x.GroupId == groupId && x.IsLeader,
            selector: x => x.StudentId);

        ArgumentNullException.ThrowIfNull(leaderId);

        return leaderId;
    }
}
