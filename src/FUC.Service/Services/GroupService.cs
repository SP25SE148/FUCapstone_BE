using System.Linq.Expressions;
using AutoMapper;
using FUC.Common.Abstractions;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FUC.Service.Services;

public class GroupService(
    IUnitOfWork<FucDbContext> uow,
    ISemesterService semesterService,
    IMapper mapper,
    IIntegrationEventLogService integrationEventLogService,
    ICurrentUser currentUser,
    ICapstoneService capstoneService) : IGroupService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Group> _groupRepository =
        uow.GetRepository<Group>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Student> _studentRepository =
        uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<GroupMember> _groupMemberRepository =
        uow.GetRepository<GroupMember>() ?? throw new ArgumentNullException(nameof(uow));

    public async Task<OperationResult<Guid>> CreateGroupAsync()
    {
        OperationResult<Semester> currentSemester = await semesterService.GetCurrentSemesterAsync();
        if (currentSemester.IsFailure)
            return OperationResult.Failure<Guid>(new Error("Error.SemesterIsNotGoingOn",
                "The current semester is not going on"));

        Student? leader = await _studentRepository.GetAsync(
            predicate: s =>
                s.Id.Equals(currentUser.UserCode) &&
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
        await _uow.BeginTransactionAsync();
        var newGroup = new Group()
        {
            CampusId = leader.CampusId,
            CapstoneId = leader.CapstoneId,
            MajorId = leader.MajorId,
            SemesterId = currentSemester.Value.Id,
        };

        _groupRepository.Insert(newGroup);

        _groupMemberRepository.Insert(new()
        {
            GroupId = newGroup.Id,
            StudentId = leader.Id,
            IsLeader = true,
            Status = GroupMemberStatus.Accepted
        });
        var groupMembers = await _groupMemberRepository.FindAsync(gm => gm.StudentId.Equals(currentUser.UserCode));
        if (groupMembers.Count > 0)
        {
            foreach (var groupMember in groupMembers)
            {
                groupMember.Status = GroupMemberStatus.Rejected;
                _groupMemberRepository.Update(groupMember);
            }
        }

        await _uow.CommitAsync();
        return newGroup.Id;
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupAsync()
    {
        var groups = await _groupRepository.GetAllAsync(
            CreateIncludeForGroupResponse(),
            g => g.OrderBy(group => group.CreatedDate),
            CreateSelectorForGroupResponse());

        return groups.Count > 0
            ? OperationResult.Success<IEnumerable<GroupResponse>>(groups)
            : OperationResult.Failure<IEnumerable<GroupResponse>>(Error.NullValue);
    }

    public async Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupBySemesterIdAsync(string semesterId)
    {
        var groups = await _groupRepository.FindAsync(
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
        var groups = await _groupRepository.FindAsync(
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
        var groups = await _groupRepository.FindAsync(
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
        var groups = await _groupRepository.FindAsync(
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
        var group = (await _groupRepository.FindAsync(g => g.Id == id,
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
        var groupMember = await _groupMemberRepository.GetAsync(
            gm => gm.StudentId.Equals(currentUser.UserCode) && gm.Status.Equals(GroupMemberStatus.Accepted),
            default);
        if (groupMember is null)
            return OperationResult.Failure<GroupResponse>(Error.NullValue);

        var group = await _groupRepository.GetAsync(g => g.Id == groupMember.GroupId,
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
            (await _groupMemberRepository.GetAsync(
                gm => gm.StudentId.Equals(currentUser.UserCode) && gm.IsLeader,
                default))?.GroupId;
        if (groupId is null)
            return OperationResult.Failure(Error.NullValue);

        var group = await _groupRepository.GetAsync(
            g => g.Id.Equals(groupId),
            true,
            g => g.Include(group => group.GroupMembers)
        );

        if (group is null)
            return OperationResult.Failure(Error.NullValue);

        if (!group.Status.Equals(GroupStatus.Pending))
            return OperationResult.Failure(new Error("Error.UpdateFailed",
                $"Can not update group status with group id {group.Id} from {group.Status.ToString()}"));

        group.Status = group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count <
                       capstone.Value.MinMember ||
                       group.GroupMembers.Where(gm => gm.Status.Equals(GroupMemberStatus.Accepted)).ToList().Count >
                       capstone.Value.MaxMember
            ? GroupStatus.Rejected
            : GroupStatus.InProgress;


        var groupCodeList = (await _groupRepository.FindAsync(g => string.IsNullOrEmpty(g.GroupCode)))
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
                } while (groupCodeList.Exists(x => x.Equals(group.GroupCode)));
            }

            await _uow.SaveChangesAsync();
            return OperationResult.Success();
        }


        return OperationResult.Success(
            $"The group with id {group.Id} just {group.Status.ToString()} because it have invalid team size");
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
}
