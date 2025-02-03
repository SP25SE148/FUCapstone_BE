using FUC.Common.Shared;
using FUC.Service.DTOs.MajorDTO;

namespace FUC.Service.Abstractions;

public interface IMajorService
{
    Task<OperationResult<string>> CreateMajorAsync(CreateMajorRequest request);
    Task<OperationResult<MajorResponse>> UpdateMajorAsync(UpdateMajorRequest request);
    Task<OperationResult<IEnumerable<MajorResponse>>> GetAllMajorsAsync();
    Task<OperationResult<IEnumerable<MajorResponse>>> GetAllActiveMajorsAsync();
    Task<OperationResult<IEnumerable<MajorResponse>>> GetMajorsByMajorGroupIdAsync(string majorGroupId);
    Task<OperationResult<MajorResponse>> GetMajorByIdAsync(string majorId);
    Task<OperationResult> DeleteMajorAsync(string majorId);
}
