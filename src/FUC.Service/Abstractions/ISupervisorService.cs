using FUC.Common.Shared;
using FUC.Service.DTOs.SupervisorDTO;

namespace FUC.Service.Abstractions;

public interface ISupervisorService
{
    Task<OperationResult<IEnumerable<SupervisorResponseDTO>>>
        GetAllSupervisorAsync(CancellationToken cancellationToken);

    Task<OperationResult<SupervisorResponseDTO>> GetSupervisorByIdAsync(string id);
}
