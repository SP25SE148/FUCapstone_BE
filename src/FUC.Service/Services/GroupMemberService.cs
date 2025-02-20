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

public class GroupMemberService(IUnitOfWork<FucDbContext> uow, IIntegrationEventLogService integrationEventLogService) : IGroupMemberService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<GroupMember> _groupMemberRepository = uow.GetRepository<GroupMember>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<Student> _studentRepository = uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<Group> _groupRepository = uow.GetRepository<Group>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IIntegrationEventLogService _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
    
    
    public async Task<OperationResult<Guid>> CreateBulkGroupMemberAsync(CreateGroupMemberRequest request)
    {
        Student? leader = await _studentRepository.GetAsync(
            predicate: s => 
                s.Id.Equals(request.LeaderId) &&
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
            return OperationResult.Failure<Guid>(new Error("Error.InEligible", $"User with id {request.LeaderId} is ineligible to add member"));

        
        
        // get number of members group's leader
        Guid groupIdOfLeader = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
        int numOfMembers = (await _groupMemberRepository
                .FindAsync(gm => gm.GroupId.Equals(groupIdOfLeader) &&
             gm.Status.Equals(GroupMemberStatus.Accepted))).Count;
            
        // Check if the team size is invalid 
        if (request.MemberIdList.Count  > leader.Capstone.MaxMember - numOfMembers)
            return OperationResult.Failure<Guid>(new Error("Error.TeamSizeInvalid", "The team size is invalid"));

        var groupMemberNotification = new List<GroupMemberNotification>();
        Guid groupId = leader.GroupMembers.FirstOrDefault(s => s.IsLeader)!.GroupId;
        // create group member for member  
        await _uow.BeginTransactionAsync();
        foreach (string memberId in request.MemberIdList)
        {
            //check if memberId value is duplicate with leaderId 
            if (memberId.Equals(leader.Id))
                return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue",$"The member id with {memberId} was duplicate with leader id {leader.Id}"));
            
            // check if member is eligible to send join group request
            Student? member = await _studentRepository.GetAsync(
                predicate: s => s.Id.Equals(memberId) && 
                                s.IsEligible &&
                                !s.Status.Equals(StudentStatus.Passed) &&
                                !s.IsDeleted,
                include: s => s.Include(s => s.GroupMembers),
                orderBy: default,
                cancellationToken: default);
            if (member is null ||
                member.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",$"Member with id {memberId} is ineligible !!"));
            
            if(member.GroupMembers.Any(s => s.GroupId == groupIdOfLeader && 
                                            s.Status.Equals(GroupMemberStatus.UnderReview)))
                return OperationResult.Failure<Guid>(new Error("Error.CreateGroupMemberFail", $"Can not send request to member with id {member.Id} because the leader already send request to this member"));
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
        IList<GroupMember>? memberRequests = await _groupMemberRepository.FindAsync(
            gm => gm.StudentId.Equals(request.MemberId),
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
            return OperationResult.Failure(new Error("Error.JoinedGroup",$"The member with id {request.MemberId} have been in gruop before!!!"));
        
        // get group member request that the member want to update status 
        GroupMember? member =  memberRequests.FirstOrDefault(gm => gm.Id == request.Id && gm.GroupId == request.GroupId);
        
        // check if the group member request is null 
        if(member is null)
            return OperationResult.Failure(new Error("Error.NotFound", $"The group member with id {request.Id} was not found !!"));
        
        // check if the group status is not in pending status
        if(!member.Group.Status.Equals(GroupStatus.Pending))
            return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not update group member status with group status is different from {GroupStatus.Pending.ToString()}"));
        
        // get the member quantities in group  
        int groupMemberQuantities = (await _groupMemberRepository.FindAsync(gm => gm.GroupId == request.GroupId && gm.Status.Equals(GroupMemberStatus.Accepted))).Count;
        
        // check if group member quantity is full
        if(groupMemberQuantities >=  member.Group.Capstone.MaxMember)
            return OperationResult.Failure(new Error("Error.ExceedGroupSlot",$"The group with id {request.GroupId} is full of member"));

        await _uow.BeginTransactionAsync();
        
        // Update status for requests
        switch (request.Status)
        {
            // Accepted - Rejected - UnderReview - LeftGroup
            case GroupMemberStatus.Accepted:
            case GroupMemberStatus.Rejected:
                if (!member.Status.Equals(GroupMemberStatus.UnderReview))
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not update from {GroupMemberStatus.Accepted.ToString()} or {GroupMemberStatus.Rejected.ToString()} to {GroupMemberStatus.Accepted.ToString()} !!!"));
                member.Status = request.Status;
                _groupMemberRepository.Update(member);
                break;
            case GroupMemberStatus.LeftGroup:
                if (!member.Status.Equals(GroupMemberStatus.Accepted))
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not left group from the other status different {GroupMemberStatus.Accepted.ToString()} status"));
                if (!member.IsLeader)
                {
                    member.Status = request.Status;
                    break;
                }
                
                // check if new leader is null
                var newLeader = await _groupMemberRepository.GetAsync(s => s.GroupId.Equals(member.GroupId) &&
                                                                           s.StudentId.ToLower().Equals(request.NewLeaderId.ToLower()) &&
                                                                           s.IsLeader == false &&
                                                                           s.Status.Equals(GroupMemberStatus.Accepted),true);
                if (newLeader is null)
                    return OperationResult.Failure(new Error("Error.NotFound", $"Can not found new leader with id {request.NewLeaderId} !!"));

                member.Status = request.Status;
                newLeader.IsLeader = true;
                _groupMemberRepository.Update(member);
                _groupMemberRepository.Update(newLeader);
                break;
            
            default:
                return OperationResult.Failure(new Error("Error.UpdateFailed", $"Can not update status with group member id {member.Id}!!")); 
            
        }
        _integrationEventLogService.SendEvent(new GroupMemberStatusUpdateMessage
        {
            AttemptTime = 1,
            CreatedBy = member.StudentId,
            Status = request.Status.ToString()
        });
        
        await _uow.CommitAsync();
        return OperationResult.Success();
    }
}
