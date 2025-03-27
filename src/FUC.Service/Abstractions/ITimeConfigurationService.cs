using FUC.Common.Shared;
using FUC.Service.DTOs.ConfigDTO;

namespace FUC.Service.Abstractions;

public interface ITimeConfigurationService
{
    Task<OperationResult<IList<TimeConfigurationDto>>> GetTimeConfigurations();
    Task<OperationResult> UpdateTimeConfiguration(UpdateTimeConfigurationRequest request,
        CancellationToken cancellationToken);
    Task<OperationResult> CreateTimeConfiguration(CreateTimeConfigurationRequest request,
        CancellationToken cancellationToken);
    Task<OperationResult<TimeConfigurationDto>> GetCurrentTimeConfiguration(string campusId);
}
