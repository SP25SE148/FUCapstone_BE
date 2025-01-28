using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.CapstoneDTO;

namespace FUC.Service.Abstractions;

public interface ICapstoneService
{
    Task<OperationResult<Guid>> CreateCapstoneAsync(CreateCapstoneRequest request);
    Task<OperationResult<CapstoneResponse>> UpdateCapstoneAsync(UpdateCapstoneRequest request);
    Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllCapstonesAsync();
    
    Task<OperationResult<IEnumerable<CapstoneResponse>>> GetAllActiveCapstonesAsync();
    Task<OperationResult<CapstoneResponse>> GetCapstoneByIdAsync(Guid capstoneId);
    Task<OperationResult> DeleteCapstoneAsync(Guid capstoneId);
}
