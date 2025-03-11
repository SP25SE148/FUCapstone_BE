using FUC.Common.Shared;
using FUC.Service.DTOs.CapstoneDTO;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicDTO;
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
    Task<OperationResult> UpdateGroupStatusAsync();
    Task<OperationResult<Guid>> CreateTopicRequestAsync(TopicRequest_Request request);
    Task<OperationResult<List<TopicRequestResponse>>> GetTopicRequestsAsync(TopicRequestParams request);
    Task<OperationResult> UpdateTopicRequestStatusAsync(UpdateTopicRequestStatusRequest request);
    Task<OperationResult<CapstoneResponse>> GetCapstoneByGroup(Guid groupId, CancellationToken cancellationToken);

    Task<bool> CheckStudentsInSameGroup(IList<string> studentIds, Guid groupId,
        CancellationToken cancellationToken);

    Task<OperationResult<GroupResponse>> GetGroupInformationByGroupSelfId();

    Task<bool> CheckSupervisorWithStudentSameGroup(IList<string> studentIds, string supervisorId,
        Guid groupId, CancellationToken cancellationToken);

    Task<OperationResult> ImportProjectProgressFile(ImportProjectProgressRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult> CreateTask(CreateTaskRequest request, CancellationToken cancellationToken);

    Task<OperationResult> CreateWeeklyEvaluation(CreateWeeklyEvaluationRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ProjectProgressDto>> GetProjectProgressByGroup(Guid groupId,
        CancellationToken cancellationToken);
}
