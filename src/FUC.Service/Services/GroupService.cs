using AutoMapper;
using Azure;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.VisualBasic;
using NetTopologySuite.Geometries;

namespace FUC.Service.Services;

public class GroupService(IUnitOfWork<FucDbContext> uow, IMapper mapper, IIntegrationEventLogService integrationEventLogService) : IGroupService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IIntegrationEventLogService _integrationEventLogService = integrationEventLogService ?? throw new ArgumentNullException(nameof(integrationEventLogService));
    private readonly IRepository<Group> _groupRepository = uow.GetRepository<Group>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IRepository<Student> _studentRepository = uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<GroupMember> _groupMemberRepository = uow.GetRepository<GroupMember>() ?? throw new ArgumentNullException(nameof(uow));
    
    public async Task<OperationResult<Guid>> CreateGroupAsync(CreateGroupRequest request, string leaderId)
    {
        Student? leader = await _studentRepository.GetAsync(
            predicate: s => 
                s.Id.Equals(leaderId) &&
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
        
        // Check if the team size is invalid 
        if (request.MembersId.Count  > leader.Capstone.MaxMember -1 ||
            request.MembersId.Count < leader.Capstone.MinMember - 1)
            return OperationResult.Failure<Guid>(new Error("Error.TeamSizeInvalid", "The team size is invalid"));

        // Check if Leader is eligible to create group 
        if (leader.Status.Equals(StudentStatus.Passed) ||
            leader.GroupMembers.Count > 0 &&
            leader.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
            return OperationResult.Failure<Guid>(new Error("Error.InEligible", "Leader is ineligible to create group"));
        
        // Create group, group member for leader
        await _uow.BeginTransactionAsync();
        var newGroup = new Group()
        {
            Id = Guid.NewGuid(),
            CampusId = request.CampusId,
            CapstoneId = request.CapstoneId,
            MajorId = request.MajorId,
            SemesterId = request.SemesterId,
            Status = GroupStatus.Pending
        };
        _groupRepository.Insert(newGroup);

        _groupMemberRepository.Insert(new()
            {
                Id = Guid.NewGuid(),
                GroupId = newGroup.Id,
                StudentId = leader.Id,
                IsLeader = true,
                Status = GroupMemberStatus.Accepted
            });

        // create groupMemberNotifications use for publish message
        var groupMemberNotifications = new List<GroupMemberNotification>();
        
        // create group member for member  
        foreach (string memberId in request.MembersId)
        {
            //check if memberId value is duplicate with leaderId 
            if (memberId.Equals(leaderId))
                return OperationResult.Failure<Guid>(new Error("Error.DuplicateValue",$"The member id with {memberId} was duplicate with leader id {leaderId}"));
            
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
                member.GroupMembers.Count > 0 &&
                member.GroupMembers.Any(s => s.Status.Equals(GroupMemberStatus.Accepted)))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",$"Member with id {memberId} is ineligible !!"));
            
            // create group member for member
            var newGroupMember = new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = newGroup.Id,
                StudentId = member.Id,
                IsLeader = false,
                Status = GroupMemberStatus.UnderReview
            };
            _groupMemberRepository.Insert(newGroupMember);
            
            groupMemberNotifications.Add(new GroupMemberNotification()
            {
                MemberId = member.Id,
                GroupId = newGroup.Id,
                GroupMemberId = newGroupMember.Id
            });
        }
        integrationEventLogService.SendEvent(new GroupMemberNotificationMessage
        {
            CreateBy = leader.Id,
            GroupMemberNotifications = groupMemberNotifications,
            LeaderEmail = leader.Email,
            LeaderName = leader.FullName,
            AttemptTime = 1
        });
        
        await _uow.CommitAsync();
        return newGroup.Id;
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupAsync()
    {
        List<Group> groups = await _groupRepository.GetAllAsync(
            CreateIncludeForGroupResponse());

         return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);

    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupBySemesterIdAsync(string semesterId)
    {
        IList<Group> groups = await _groupRepository.FindAsync(
            g => g.SemesterId == semesterId,
            CreateIncludeForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups.ToList()))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByMajorIdAsync(string majorId)
    {
        IList<Group> groups = await _groupRepository.FindAsync(
            g => g.MajorId == majorId,
            CreateIncludeForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups.ToList()))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCapstoneIdAsync(string capstoneId)
    {
        IList<Group> groups = await _groupRepository.FindAsync(
            g => g.CapstoneId == capstoneId,
            CreateIncludeForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups.ToList()))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCampusIdAsync(string campusId)
    {
        IList<Group> groups = await _groupRepository.FindAsync(
            g => g.CampusId == campusId,
            CreateIncludeForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups.ToList()))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<GroupResponse>> GetGroupByIdAsync(Guid id)
    {
        Group? group = await _groupRepository.GetAsync(g => g.Id == id,
            false,
            CreateIncludeForGroupResponse(),
            cancellationToken: default);
        return group is null
            ? OperationResult.Failure<GroupResponse>(Error.NullValue)
            : OperationResult.Success(_mapper.Map<GroupResponse>(group));
    }

    public Task<OperationResult> UpdateGroupStatusAsync()
    {
        throw new NotImplementedException();
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
    
}
