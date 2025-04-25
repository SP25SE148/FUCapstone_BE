using AutoMapper;
using FUC.Common.Abstractions;
using FUC.Common.Shared;
using FUC.Data;
using FUC.Data.Data;
using FUC.Data.Entities;
using FUC.Data.Repositories;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.StudentDTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FUC.Service.Services;

public sealed class StudentService(
    IMapper mapper,
    ICurrentUser currentUser,
    IRepository<Student> studentRepository,
    IRepository<Group> groupRepository,
    IUnitOfWork<FucDbContext> uow) : IStudentService
{
    public async Task<OperationResult<IEnumerable<StudentResponseDTO>>> GetAllStudentAsync(
        CancellationToken cancellationToken)
    {
        var students = await studentRepository.FindAsync(
            x => (currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId) &&
                 (currentUser.MajorId == "all" || x.MajorId == currentUser.MajorId) &&
                 (currentUser.CapstoneId == "all" || x.CapstoneId == currentUser.CapstoneId),
            CreateIncludeForStudentResponse(),
            x => x.OrderBy(x => x.Id),
            cancellationToken);
        return OperationResult.Success(mapper.Map<IEnumerable<StudentResponseDTO>>(students));
    }

    public async Task<OperationResult<IList<StudentResponseDTO>>> GetRemainStudentsAsync(
        CancellationToken cancellationToken)
    {
        var students = await studentRepository.FindAsync(
            predicate: x => (currentUser.CampusId == "all" || x.CampusId == currentUser.CampusId) &&
                            (currentUser.MajorId == "all" || x.MajorId == currentUser.MajorId) &&
                            (currentUser.CapstoneId == "all" || x.CapstoneId == currentUser.CapstoneId) &&
                            !x.GroupMembers.Any(x => x.Status == Data.Enums.GroupMemberStatus.Accepted),
            include: CreateIncludeForStudentResponse(),
            orderBy: x => x.OrderBy(x => x.Id),
            selector: x => new StudentResponseDTO
            {
                Id = x.Id,
                FullName = x.FullName,
                MajorId = x.MajorId,
                MajorName = x.Major.Name,
                CapstoneId = x.CapstoneId,
                CapstoneName = x.Capstone.Name,
                CampusId = x.CampusId,
                CampusName = x.Campus.Name,
                Email = x.Email,
                Status = x.Status.ToString(),
                Skills = x.Skills ?? "UnDefined",
                Gpa = x.GPA,
                IsHaveBeenJoinGroup = false,
            },
            cancellationToken);

        return OperationResult.Success(students);
    }

    public async Task<OperationResult<StudentResponseDTO>> GetStudentByIdAsync(string id)
    {
        Student? student = await studentRepository.GetAsync(s => s.Id.ToLower().Equals(id.ToLower()),
            true,
            CreateIncludeForStudentResponse());
        return student is not null
            ? mapper.Map<StudentResponseDTO>(student)
            : OperationResult.Failure<StudentResponseDTO>(Error.NullValue);
    }

    public async Task<OperationResult> UpdateStudentInformation(UpdateStudentRequest request, string studentId)
    {
        var student = await studentRepository.GetAsync(s => s.Id.ToLower().Equals(studentId.ToLower()),
            true,
            CreateIncludeForStudentResponse());

        // check if student is null
        if (student is null)
            return OperationResult.Failure(new Error("Error.NotFound",
                $"Student with id {studentId} is not found !!!"));

        // update student info
        if (request.BusinessAreaId != null)
            student.BusinessAreaId = request.BusinessAreaId;
        if (request.GPA != null)
            student.GPA = (float)request.GPA;
        student.Skills = request.Skills ?? default;

        var groupMember = student.GroupMembers.FirstOrDefault(x => x.Status == Data.Enums.GroupMemberStatus.Accepted);

        if (groupMember != null)
        {
            var group = await groupRepository.GetAsync(
                x => x.Id == groupMember.GroupId,
                true,
                include: x => x
                    .Include(x => x.GroupMembers.Where(x => x.Id != groupMember.Id))
                    .ThenInclude(x => x.Student));

            ArgumentNullException.ThrowIfNull(group);

            var sumGpaOtherStudents = group.GroupMembers.Sum(x => x.Student.GPA);
            var numberOfStudent = group.GroupMembers.Count(x => x.Student.GPA != 0);

            if (request.GPA != null)
                group.GPA = (sumGpaOtherStudents + (float)request.GPA) / (numberOfStudent + 1);
        }

        await uow.SaveChangesAsync();

        return OperationResult.Success();
    }

    private static Func<IQueryable<Student>, IIncludableQueryable<Student, object?>> CreateIncludeForStudentResponse()
    {
        return s => s
            .Include(s => s.Major)
            .Include(s => s.Capstone)
            .Include(s => s.Campus)
            .Include(s => s.GroupMembers)
            .Include(s => s.BusinessArea);
    }

    public async Task<OperationResult<IList<InviteStudentsResponseDto>>> GetStudentsForInvitation(string searchTerm,
        CancellationToken cancellationToken = default)
    {
        var students = await studentRepository.FindAsync(
            x => x.CampusId == currentUser.CampusId &&
                 x.MajorId == currentUser.MajorId &&
                 (string.IsNullOrEmpty(searchTerm) || x.Email.ToLower().Contains(searchTerm.ToLower())),
            include: null,
            orderBy: null,
            selector: x => new InviteStudentsResponseDto
            {
                Email = x.Email,
                Id = x.Id,
            },
            cancellationToken: cancellationToken);

        return OperationResult.Success(students);
    }
}
