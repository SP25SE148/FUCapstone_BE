﻿using AutoMapper;
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
                BusinessArea = x.BusinessArea.Name,
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
        student.BusinessAreaId = request.BusinessAreaId;
        student.GPA = request.GPA;

        var groupMember = student.GroupMembers.FirstOrDefault(x => x.Status == Data.Enums.GroupMemberStatus.Accepted);

        if (groupMember != null)
        {
            var group = await groupRepository.GetAsync(
                x => x.Id == groupMember.GroupId, 
                include: x => x
                        .Include(x => x.GroupMembers.Where(x => x.Id != groupMember.Id))
                        .ThenInclude(x => x.Student),
                default);

            ArgumentNullException.ThrowIfNull(group); 

            var sumGpaOtherStudents = group.GroupMembers.Sum(x => x.Student.GPA);
            var numberOfStudent = group.GroupMembers.Count(x => x.Student.GPA != 0);

            group.GPA = (sumGpaOtherStudents + request.GPA) / (numberOfStudent + 1);

            groupRepository.Update(group);  
        }

        studentRepository.Update(student);

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
}
