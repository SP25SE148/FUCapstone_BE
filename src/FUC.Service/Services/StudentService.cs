using AutoMapper;
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

public sealed class StudentService(IMapper mapper, IUnitOfWork<FucDbContext> uow) : IStudentService
{
    private readonly IUnitOfWork<FucDbContext> _uow = uow ?? throw new ArgumentNullException(nameof(uow));

    private readonly IRepository<Student> _studentRepository = uow.GetRepository<Student>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IRepository<StudentExpertise> _studentExpertiseRepository = uow.GetRepository<StudentExpertise>() ?? throw new ArgumentNullException(nameof(uow));
    private readonly IMapper _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    
    public async Task<OperationResult<IEnumerable<StudentResponseDTO>>> GetAllStudentAsync()
    {
        List<Student> students = await _studentRepository
            .GetAllAsync(CreateIncludeForStudentResponse());
        return students.Count > 0
            ? OperationResult.Success(_mapper.Map<IEnumerable<StudentResponseDTO>>(students))
            : OperationResult.Failure<IEnumerable<StudentResponseDTO>>(Error.NullValue);
    }

    public async Task<OperationResult<StudentResponseDTO>> GetStudentByIdAsync(string id)
    {
        Student? student = await _studentRepository.GetAsync(s => s.Id.ToLower().Equals(id.ToLower()),
                true,
                CreateIncludeForStudentResponse());
        return student is not null
            ? _mapper.Map<StudentResponseDTO>(student)
            : OperationResult.Failure<StudentResponseDTO>(Error.NullValue);
    }

    public async Task<OperationResult> UpdateStudentInformation(UpdateStudentRequest request, string studentId)
    {
        var student = await _studentRepository.GetAsync(s => s.Id.ToLower().Equals(studentId.ToLower()),
            true,
            CreateIncludeForStudentResponse());
        
        // check if student is null
        if(student is null)
            return OperationResult.Failure(new Error("Error.NotFound", $"Student with id {studentId} is not found !!!"));

        // update student info
        await _uow.BeginTransactionAsync();
        student.BusinessAreaId = request.BusinessAreaId;
        if (student.StudentExpertises.Count < 1)
        {
            foreach (var studentExpertise  in request.StudentExpertises)
            {
               _studentExpertiseRepository.Insert(new StudentExpertise
               {
                   Id = Guid.NewGuid(),
                   StudentId = student.Id,
                   TechnicalAreaId = studentExpertise.TechnicalAreaId
               }); 
            }    
        }
        else
        {
            foreach (var studentExpertise in request.StudentExpertises)
            {
                var result = student.StudentExpertises.FirstOrDefault(se => se.Id.Equals(studentExpertise.Id));
                if(result is null)
                    return OperationResult.Failure(new Error("Error.UpdateFailed",$"Can not update student expertise with id {studentExpertise.Id}"));
                result.TechnicalAreaId = studentExpertise.TechnicalAreaId;
            }            
        }

        _studentRepository.Update(student);
        await _uow.CommitAsync();
        return OperationResult.Success();
    }

    private static Func<IQueryable<Student>, IIncludableQueryable<Student, object>> CreateIncludeForStudentResponse()
    {
        return s => s
            .Include(s => s.Major)
            .Include(s => s.Capstone)
            .Include(s => s.Campus)
            .Include(s => s.GroupMembers)
            .Include(s => s.BusinessArea)
            .Include(s => s.StudentExpertises)
            .ThenInclude(se => se.TechnicalArea);
    }
}
