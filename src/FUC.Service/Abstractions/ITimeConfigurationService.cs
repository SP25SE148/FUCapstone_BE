using FUC.Common.Shared;
using FUC.Service.DTOs.ConfigDTO;

namespace FUC.Service.Abstractions;

public interface ITimeConfigurationService
{
    Task<OperationResult<IList<TimeConfigurationDto>>> GetTimeConfigurations();
    Task<OperationResult<TimeConfigurationDto>> GetTimeConfigurationBySemesterId(string semesterId);

    Task<OperationResult> UpdateTimeConfiguration(UpdateTimeConfigurationRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult> CreateTimeConfiguration(CreateTimeConfigurationRequest request,
        CancellationToken cancellationToken);
}
