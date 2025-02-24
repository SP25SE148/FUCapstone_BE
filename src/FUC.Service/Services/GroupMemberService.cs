using System.Runtime.InteropServices.JavaScript;
using AutoMapper;
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

namespace FUC.Service.Services;

public class GroupMemberService(IUnitOfWork<FucDbContext> uow,
    IIntegrationEventLogService integrationEventLogService,
    ICurrentUser currentUser,
    IMapper mapper) : IGroupMemberService

{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<GroupMember> _groupMemberRepository = uow.GetRepository<GroupMember>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<Student> _studentRepository = uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IIntegrationEventLogService _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
    
    
    public async Task<OperationResult<Guid>> CreateBulkGroupMemberAsync(CreateGroupMemberRequest request)
    {
        Student? leader = await _studentRepository.GetAsync(
            predicate: s => 
                s.Id.Equals(currentUser.UserCode) &&
                s.IsEligible &&
                !s.IsDeleted &&
                !s.Status.Equals(StudentStatus.Passed),
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
            return OperationResult.Failure<Guid>(new Error("Error.InEligible", $"User with id {currentUser.UserCode} is ineligible to add member"));

        
        
        // get number of members group's leader
        Guid groupIdOfLeader = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
        int numOfMembers = (await _groupMemberRepository
                .FindAsync(gm => gm.GroupId.Equals(groupIdOfLeader) &&
             gm.Status.Equals(GroupMemberStatus.Accepted))).Count;
            
        // Check if the team size is invalid 
        if (request.MemberEmailList.Count  > leader.Capstone.MaxMember - numOfMembers)
            return OperationResult.Failure<Guid>(new Error("Error.TeamSizeInvalid", "The team size is invalid"));

        var groupMemberNotification = new List<GroupMemberNotification>();
        Guid groupId = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
        // create group member for member  
        await _uow.BeginTransactionAsync();
        foreach (string memberEmail in request.MemberEmailList)
        {
            //check if memberId value is duplicate with leaderId 
            if (memberEmail.Equals(leader.Id))
                return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue",$"The member id with {memberEmail} was duplicate with leader id {leader.Id}"));
            
            // check if member is eligible to send join group request
            Student? member = await _studentRepository.GetAsync(
                predicate: s => s.Email.ToLower().Equals(memberEmail.ToLower()) && 
                                s.IsEligible &&
                                !s.Status.Equals(StudentStatus.Passed) &&
                                !s.IsDeleted,
                include: s => s.Include(s => s.GroupMembers),
                orderBy: default,
                cancellationToken: default);
            if (member is null ||
                member.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",$"Member with email {memberEmail} is ineligible !!"));
            
            if(member.GroupMembers.Any(s => s.GroupId == groupIdOfLeader && 
                                            s.Status.Equals(GroupMemberStatus.UnderReview)))
                return OperationResult.Failure<Guid>(new Error("Error.CreateGroupMemberFail", $"Can not send request to member with email {member.Email} because the leader already send request to this member"));
            // create group member for member
            var newGroupMember = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = groupId,
                StudentId = member.Id,
                IsLeader = false,
                Status = GroupMemberStatus.UnderReview
            };
            
            _groupMemberRepository.Insert(newGroupMember);
            
            groupMemberNotification.Add(new GroupMemberNotification
            {
                GroupId = groupId,
                MemberEmail = member.Email,
                MemberId = member.Id,
                GroupMemberId = newGroupMember.Id   
            });
        }
        _integrationEventLogService.SendEvent(new GroupMemberNotificationMessage
        {
            GroupMemberNotifications = groupMemberNotification,
            CreateBy = leader.Id,
            LeaderEmail = leader.Email,
            LeaderName = leader.FullName,
            AttemptTime = 1
        });
        await _uow.CommitAsync();
        return groupId;
    }

    public async Task<OperationResult> UpdateGroupMemberStatusAsync(UpdateGroupMemberRequest request)
    {
        // get all request of member
        string? memberRequestId = !request.Status.Equals(GroupMemberStatus.Cancelled) ? currentUser.UserCode : request.MemberId;
        IList<GroupMember>? memberRequests = await _groupMemberRepository.FindAsync(
            gm => gm.StudentId.ToLower().Equals(memberRequestId.ToLower()),
            gm => gm.Include(grm => grm.Group)
                .ThenInclude(g => g.Capstone),
            isEnabledTracking: true,
            cancellationToken: default);
        
        
        // check if member requests is null
        if(memberRequests.Count < 1)
            return OperationResult.Failure(Error.NullValue);

        // check if member have been in group before
        bool memberHaveBeenInGroupBefore = memberRequests.Any(gm => gm.Status.Equals(GroupMemberStatus.Accepted));
        
        if(memberHaveBeenInGroupBefore)
            return OperationResult.Failure(new Error("Error.JoinedGroup",$"The member with id {currentUser.UserCode} have been in gruop before!!!"));
        
        // get group member request that the member want to update status 
        GroupMember? groupMember =  memberRequests.FirstOrDefault(gm => gm.Id == request.Id && gm.GroupId == request.GroupId);
        
        // check if the group member request is null 
        if(groupMember is null)
            return OperationResult.Failure(new Error("Error.NotFound", $"The group member with id {request.Id} was not found !!"));
        
        // check if the group status is not in pending status
        if(!groupMember.Group.Status.Equals(GroupStatus.Pending))
            return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not update group member status with group status is different from {GroupStatus.Pending.ToString()}"));
        
        // get the member quantities in group  
        int groupMemberQuantities = (await _groupMemberRepository.FindAsync(gm => gm.GroupId == request.GroupId && gm.Status.Equals(GroupMemberStatus.Accepted))).Count;
        
        // check if group member quantity is full
        if(groupMemberQuantities >=  groupMember.Group.Capstone.MaxMember)
            return OperationResult.Failure(new Error("Error.ExceedGroupSlot",$"The group with id {request.GroupId} is full of member"));

        await _uow.BeginTransactionAsync();
        
        // Update status for requests
        switch (request.Status)
        {
            // Accepted - Rejected - UnderReview - LeftGroup
            case GroupMemberStatus.Accepted:
            case GroupMemberStatus.Rejected:
                if (!groupMember.Status.Equals(GroupMemberStatus.UnderReview))
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not update from {GroupMemberStatus.Accepted.ToString()} or {GroupMemberStatus.Rejected.ToString()} to {GroupMemberStatus.Accepted.ToString()} !!!"));
                groupMember.Status = request.Status;
                _groupMemberRepository.Update(groupMember);
                if (request.Status.Equals(GroupMemberStatus.Accepted))
                {
                    foreach (var memberRequest in memberRequests.Where(gm => gm.Id != request.Id).ToList())
                    {
                        memberRequest.Status = GroupMemberStatus.Rejected;
                        _groupMemberRepository.Update(memberRequest);
                    }
                }
                break;
            case GroupMemberStatus.LeftGroup:
                if (!groupMember.Status.Equals(GroupMemberStatus.Accepted))
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not left group from the other status different {GroupMemberStatus.Accepted.ToString()} status"));
                if (!groupMember.IsLeader)
                {
                    groupMember.Status = request.Status;
                    break;
                }
                
                // check if new leader is null
                var newLeader = await _groupMemberRepository.GetAsync(s => s.GroupId.Equals(groupMember.GroupId) &&
                                                                           s.StudentId.ToLower().Equals(request.NewLeaderId.ToLower()) &&
                                                                           s.IsLeader == false &&
                                                                           s.Status.Equals(GroupMemberStatus.Accepted),true);
                if (newLeader is null)
                    return OperationResult.Failure(new Error("Error.NotFound", $"Can not found new leader with id {request.NewLeaderId} !!"));

                groupMember.Status = request.Status;
                newLeader.IsLeader = true;
                _groupMemberRepository.Update(groupMember);
                _groupMemberRepository.Update(newLeader);
                break;
            case GroupMemberStatus.Cancelled:
                var groupMemberLeader = await _groupMemberRepository.GetAsync(
                    gm => gm.GroupId.Equals(request.GroupId) && gm.StudentId.Equals(currentUser.UserCode),
                    default); 
                if(groupMemberLeader is null || !groupMemberLeader.IsLeader)
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"The Group Member with group id {request.GroupId} && student id {currentUser.UserCode} was not found or was not leader!"));

                groupMember.Status = GroupMemberStatus.Cancelled;
                _groupMemberRepository.Update(groupMember);
                break;
            default:
                return OperationResult.Failure(new Error("Error.UpdateFailed", $"Can not update status with group member id {groupMember.Id}!!")); 
            
        }
        _integrationEventLogService.SendEvent(new GroupMemberStatusUpdateMessage
        {
            AttemptTime = 1,
            CreatedBy = groupMember.StudentId,
            Status = request.Status.ToString()
        });
        
        await _uow.CommitAsync();
        return OperationResult.Success();
    }

    public async Task<OperationResult<GroupMemberRequestResponse>> GetGroupMemberRequestByMemberId()
    {
        var groupMemberRequestResponse = new GroupMemberRequestResponse();
        
        IList<GroupMember> groupMembers = await _groupMemberRepository.FindAsync(
            gm => gm.StudentId.Equals(currentUser.UserCode),
            gm => gm.Include(gm => gm.Student),
            x => x.OrderBy(x => x.CreatedDate));
        
        if (groupMembers.Count < 1)
            return OperationResult.Failure<GroupMemberRequestResponse>(Error.NullValue);

        groupMemberRequestResponse.GroupMemberRequested = groupMembers.Where(gm => !gm.IsLeader).Select(
            x => new GroupMemberResponse()
            {
                Id = x.Id,
                Status = x.Status.ToString(),
                GroupId = x.GroupId,
                StudentId = x.StudentId,
                StudentFullName = x.Student.FullName,
                StudentEmail = x.Student.Email,
                IsLeader = x.IsLeader
            }).ToList();
        if (groupMembers.Any(gm => gm.IsLeader))
        {
            IList<GroupMemberResponse> groupMembersRequestOfLeader =
                await _groupMemberRepository.FindAsync(
                    gm => gm.CreatedBy.Equals(currentUser.Email) && !gm.IsLeader,
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
                        IsLeader = x.IsLeader
                    });
            if(groupMembersRequestOfLeader.Count < 1)
                return OperationResult.Failure<GroupMemberRequestResponse>(Error.NullValue);
            
            groupMemberRequestResponse.GroupMemberRequestSentByLeader = groupMembersRequestOfLeader.ToList();
        }
        return OperationResult.Success(groupMemberRequestResponse);
        
    }
    
}
