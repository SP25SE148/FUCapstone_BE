using FUC.Common.Shared;
using FUC.Service.DTOs.GroupDTO;

namespace FUC.Service.Abstractions;

public interface IGroupService
{
    Task<OperationResult<Guid>> CreateGroupAsync(CreateGroupRequest request,string leaderId);
}
