﻿using FUC.Common.Contracts;
using FUC.Common.Shared;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.TopicRequestDTO;

namespace FUC.Service.Abstractions;

public interface IGroupService
{
    Task<OperationResult<Guid>> CreateGroupAsync();
    Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupAsync();
    Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupBySemesterIdAsync(string semesterId);
    Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByMajorIdAsync(string majorId);
    Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCapstoneIdAsync(string capstoneId);
    Task<OperationResult<IEnumerable<GroupResponse>>> GetAllGroupByCampusIdAsync(string campusId);
    Task<OperationResult<GroupResponse>> GetGroupByIdAsync(Guid id);
    Task<OperationResult<GroupResponse>> GetGroupByStudentIdAsync();
    Task<OperationResult> UpdateGroupStatusAsync();
    Task<OperationResult<Guid>> CreateTopicRequest(TopicRequest_Request request);
}
