using AutoMapper;
using DocumentFormat.OpenXml.Office2010.ExcelAc;
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

namespace FUC.Service.Services;

public class GroupService(IUnitOfWork<FucDbContext> uow, IMapper mapper, IPublishEndpoint publishEndpoint) : IGroupService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<Group> _groupRepository = uow.GetRepository<Group>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    private readonly IRepository<Student> _studentRepository = uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<GroupMember> _groupMemberRepository = uow.GetRepository<GroupMember>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));

    public async Task<OperationResult<Guid>> CreateGroupAsync(CreateGroupRequest request, string leaderId)
    {
        Student? leader = await _studentRepository.GetAsync(
            predicate: s => 
                s.Id.Equals(leaderId) &&
                s.IsEligible &&
                !s.IsDeleted,
            include: s => 
                s.Include(s => s.GroupMember)
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
            leader.GroupMember != null &&
            leader.GroupMember.Status.Equals(GroupMemberStatus.Accepted))
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
                include: s => s.Include(s => s.GroupMember),
                orderBy: default,
                cancellationToken: default);
            if (member is null ||
                member.GroupMember != null &&
                member.GroupMember.Status.Equals(GroupMemberStatus.Accepted))
                return OperationResult.Failure<Guid>(new Error("Error.InEligible",$"Member with id {memberId} is ineligible !!"));
            
            // create group member for member
            _groupMemberRepository.Insert(new GroupMember
            {
                Id = Guid.NewGuid(),
                GroupId = newGroup.Id,
                StudentId = member.Id,
                IsLeader = false,
                Status = GroupMemberStatus.UnderReview
            });
        }
        await _uow.CommitAsync();
        return newGroup.Id;
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupAsync()
    {
        List<Group> groups = await _groupRepository.GetAllAsync(
            g => 
                g.Include(g => g.GroupMembers)
                .ThenInclude(gm => gm.Student)
                .Include(g => g.Major)
                .Include(g => g.Semester)
                .Include(g => g.Capstone)
                .Include(g => g.Campus));

         return groups.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<GroupResponse>>(groups))
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);

    }

    public Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupBySemesterIdAsync(string semesterId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByMajorIdAsync(string majorId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCapstoneIdAsync(string capstoneId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<GroupResponse>>> GetGroupByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}
