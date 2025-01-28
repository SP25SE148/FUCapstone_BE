using FUC.Common.Shared;
using FUC.Service.DTOs.MajorGroupDTO;

namespace FUC.Service.Abstractions;

public interface IMajorGroupService
{
    Task<OperationResult<Guid>> CreateMajorGroupAsync(CreateMajorGroupRequest request);
    Task<OperationResult<MajorGroupResponse>> UpdateMajorGroupAsync(UpdateMajorGroupRequest request);
    Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllMajorGroupsAsync();
    Task<OperationResult<IEnumerable<MajorGroupResponse>>> GetAllActiveMajorGroupsAsync();
    Task<OperationResult<MajorGroupResponse>> GetMajorGroupByIdAsync(Guid majorGroupId);
    Task<OperationResult> DeleteMajorGroupAsync(Guid majorGroupId);    
}
