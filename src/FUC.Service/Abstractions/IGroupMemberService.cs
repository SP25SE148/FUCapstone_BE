using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.GroupMemberDTO;

namespace FUC.Service.Abstractions;

public interface IGroupMemberService
{
    Task<OperationResult<Guid>> CreateBulkGroupMemberAsync(CreateGroupMemberRequest request);
    Task<OperationResult> UpdateGroupMemberStatusAsync(UpdateGroupMemberRequest request);
    Task<OperationResult<GroupMemberRequestResponse>> GetGroupMemberRequestByMemberId();
    Task<OperationResult<IEnumerable<GroupMember>>> GetGroupMemberByGroupId(Guid groupId);
}
