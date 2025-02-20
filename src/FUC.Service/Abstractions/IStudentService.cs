using FUC.Common.Shared;
using FUC.Service.DTOs.StudentDTO;

namespace FUC.Service.Abstractions;

public interface IStudentService
{
    Task<OperationResult<IEnumerable<StudentResponseDTO>>> GetAllStudentAsync();
    Task<OperationResult<StudentResponseDTO>> GetStudentByIdAsync(string id);
}
