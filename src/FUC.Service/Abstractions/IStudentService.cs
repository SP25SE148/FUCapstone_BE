using FUC.Common.Shared;
using FUC.Service.DTOs.StudentDTO;

namespace FUC.Service.Abstractions;

public interface IStudentService
{
    Task<OperationResult<IEnumerable<StudentResponseDTO>>> GetAllStudentAsync(CancellationToken cancellationToken);
    Task<OperationResult<StudentResponseDTO>> GetStudentByIdAsync(string id);
    Task<OperationResult> UpdateStudentInformation(UpdateStudentRequest request, string studentId);
    Task<OperationResult<IList<StudentResponseDTO>>> GetRemainStudentsAsync(
       CancellationToken cancellationToken);
    Task<OperationResult<IList<InviteStudentsResponseDto>>> GetStudentsForInvitation(string searchTerm, CancellationToken cancellationToken = default);
}
