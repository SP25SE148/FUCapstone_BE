﻿using FUC.Common.Shared;
using FUC.Data.Entities;
using FUC.Service.DTOs.GroupMemberDTO;

namespace FUC.Service.Abstractions;

public interface IGroupMemberService
{
    Task<OperationResult<Guid>> CreateGroupMemberByLeaderAsync(CreateGroupMemberByLeaderRequest request);
    Task<OperationResult> UpdateGroupMemberStatusAsync(UpdateGroupMemberRequest request);
    Task<OperationResult<GroupMemberRequestResponse>> GetGroupMemberRequestByMemberId();
    Task<OperationResult<Guid>> CreateJoinGroupRequestByMemberAsync(CreateJoinGroupRequestByMember request);
    Task<OperationResult> UpdateJoinGroupRequestAsync(UpdateJoinGroupRequest request);
    Task<OperationResult<IEnumerable<GroupMember>>> GetGroupMemberByGroupId(Guid groupId);
}
