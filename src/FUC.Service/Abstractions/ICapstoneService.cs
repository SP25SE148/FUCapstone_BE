using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.CapstoneDTO;

namespace FUC.Service.Abstractions;

public interface ICapstoneService
{
    Task<OperationResult<string>> CreateCapstoneAsync(CreateCapstoneRequest request);
    Task<OperationResult<CapstoneResponse>> UpdateCapstoneAsync(UpdateCapstoneRequest request);
    Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllCapstonesAsync();

    Task<OperationResult<IEnumerable<CapstoneResponse>>> GetCapstonesByMajorIdAsync(string majorId);
    Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllActiveCapstonesAsync();
    Task<OperationResult<CapstoneResponse>> GetCapstoneByIdAsync(string capstoneId);
    Task<OperationResult> DeleteCapstoneAsync(string capstoneId);
}
