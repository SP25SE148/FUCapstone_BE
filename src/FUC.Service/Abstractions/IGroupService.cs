﻿using FUC.Common.Shared;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
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
    Task<OperationResult<Guid>> CreateTopicRequestAsync(TopicRequest_Request request);
    Task<OperationResult<List<TopicRequestResponse>>> GetTopicRequestsAsync(TopicRequestParams request);
    Task<OperationResult> UpdateTopicRequestStatusAsync(UpdateTopicRequestStatusRequest request);
    Task<OperationResult> ImportProjectProgressFile(ImportProjectProgressRequest request, CancellationToken cancellationToken);
    Task<OperationResult> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken);
    Task<OperationResult> UpdateTask(UpdateTaskRequest request, CancellationToken cancellationToken);
    Task<OperationResult<List<FucTaskResponse>>> GetTasks(Guid projectProgressId, CancellationToken cancellationToken);
    Task<OperationResult<FucTaskDetailResponse>> GetTasksDetail(Guid taskId, CancellationToken cancellationToken);
    Task<OperationResult> CreateWeeklyEvaluation(CreateWeeklyEvaluationRequest request, CancellationToken cancellationToken);
    Task<OperationResult<ProjectProgressDto>> GetProjectProgressByGroup(Guid groupId, CancellationToken cancellationToken);
    Task<OperationResult<List<EvaluationProjectProgressResponse>>> GetProgressEvaluationOfGroup(Guid groupId, CancellationToken cancellationToken);
    Task<OperationResult<byte[]>> ExportProgressEvaluationOfGroup(Guid groupId, CancellationToken cancellationToken);
    Task<OperationResult<TopicOfGroupResponse>> GetGroupInformationByGroupSelfId();
    Task<OperationResult<List<GroupManageBySupervisorResponse>>> GetGroupsWhichMentorBySupervisor(CancellationToken cancellationToken);
}
