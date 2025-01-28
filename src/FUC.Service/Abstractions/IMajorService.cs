using FUC.Common.Shared;
using FUC.Service.DTOs.MajorDTO;

namespace FUC.Service.Abstractions;

public interface IMajorService
{
    Task<OperationResult<Guid>> CreateMajorAsync(CreateMajorRequest request);
    Task<OperationResult<MajorResponse>> UpdateMajorAsync(UpdateMajorRequest request);
    Task<OperationResult<IEnumerable<MajorResponse>>> GetAllMajorsAsync();
    Task<OperationResult<IEnumerable<MajorResponse>>> GetAllActiveMajorsAsync();
    Task<OperationResult<IEnumerable<MajorResponse>>> GetMajorsByGroupIdAsync(Guid majorGroupId);
    Task<OperationResult<MajorResponse>> GetMajorByIdAsync(Guid majorId);
    Task<OperationResult> DeleteMajorAsync(Guid majorId);
}
