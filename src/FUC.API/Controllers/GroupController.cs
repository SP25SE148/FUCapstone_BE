using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Data.Enums;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using FUC.Service.DTOs.ProjectProgressDTO;
using FUC.Service.DTOs.TopicDTO;
using FUC.Service.DTOs.TopicRequestDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public class GroupController(
    IGroupService groupService,
    IGroupMemberService groupMemberService,
    ITopicService topicService) : ApiController
{
    #region Group Endpoint

    [HttpPost]
    [Authorize(Roles = nameof(UserRoles.Student))]
    public async Task<IActionResult> CreateGroupAsync()
    {
        OperationResult<Guid> result = await groupService.CreateGroupAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("decision/{groupId}")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetGroupDecisionAsync(Guid groupId)
    {
        var result = await groupService.GetGroupDecisionByGroupIdAsync(groupId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut]
    [Authorize(Roles = nameof(UserRoles.Student))]
    public async Task<IActionResult> CreateGroupCodeAsync()
    {
        var result = await groupService.UpdateGroupStatusAsync();

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public async Task<IActionResult> GetGroups()
    {
        var result = await groupService.GetAllGroupAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-semester-id/{semesterId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsBySemesterId(string semesterId)
    {
        var result = await groupService.GetAllGroupBySemesterIdAsync(semesterId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-major-id/{majorId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsByMajorId(string majorId)
    {
        var result = await groupService.GetAllGroupByMajorIdAsync(majorId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-capstone-id/{capstoneId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsByCapstoneId(string capstoneId)
    {
        var
            result = await groupService.GetAllGroupByCapstoneIdAsync(capstoneId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("pending")]
    [Authorize(Roles = $"{UserRoles.Student}")]
    public async Task<IActionResult> GetPendingGroupsForStudentJoin()
    {
        var result = await groupService.GetPendingGroupsForStudentJoin(default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetGroupByIdAsync(Guid id)
    {
        var result = await groupService.GetGroupByIdAsync(id, IsCheckAccess: true);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-campus-id/{campusId}")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public async Task<IActionResult> GetGroupByCampusIdAsync(string campusId)
    {
        var result = await groupService.GetAllGroupByCampusIdAsync(campusId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("merge/remain")]
    [Authorize(Roles = nameof(UserRoles.Manager))]
    public async Task<IActionResult> MergeGroupForRemainStudents()
    {
        var result = await groupService.MergeGroupForRemainStudents(default);

        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("assign/remain")]
    [Authorize(Roles = nameof(UserRoles.Manager))]
    public async Task<IActionResult> AssignRemainStudentForGroup(AssignRemainStudentForGroupRequest request)
    {
        var result = await groupService.AssignRemainStudentForGroup(request, default);

        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("assign/pending-topic")]
    [Authorize(Roles = nameof(UserRoles.Manager))]
    public async Task<IActionResult> AssignPendingTopicForGroup(AssignPendingTopicForGroupRequest request)
    {
        var result = await groupService.AssignPendingTopicForGroup(request, default);

        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    #endregion

    #region Group Member Endpoint

    [Authorize(Roles = nameof(UserRoles.Student))]
    [HttpPost("add-member")]
    public async Task<IActionResult> AddMemberIntoGroupAsync(CreateGroupMemberByLeaderRequest request)
    {
        OperationResult<Guid> result = await groupMemberService.CreateGroupMemberByLeaderAsync(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("join-group-request")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> CreateJoinGroupRequestAsync([FromBody] CreateJoinGroupRequestByMember request)
    {
        var result = await groupMemberService.CreateJoinGroupRequestByMemberAsync(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut("update-join-group-request")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> UpdateJoinGroupRequestAsync([FromBody] UpdateJoinGroupRequest request)
    {
        var result = await groupMemberService.UpdateJoinGroupRequestAsync(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.Student))]
    [HttpPut("update-group-member-status")]
    public async Task<IActionResult> UpdateGroupMemberStatusAsync(UpdateGroupMemberRequest request)
    {
        OperationResult result = await groupMemberService.UpdateGroupMemberStatusAsync(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [Authorize(Roles = nameof(UserRoles.Student))]
    [HttpGet("student/get-group-member-request")]
    public async Task<IActionResult> GetGroupMemberRequest()
    {
        var result = await groupMemberService.GetGroupMemberRequestByMemberId();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    #endregion

    #region TopicRequest

    [HttpPost("create-topic-request")]
    [Authorize(Roles = $"{UserRoles.Student}")]
    public async Task<IActionResult> CreateTopicRequest(TopicRequest_Request request)
    {
        var result = await groupService.CreateTopicRequestAsync(request, default);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-topic-request")]
    [Authorize(Roles = $"{UserRoles.Student}, {UserRoles.Supervisor}")]
    public async Task<IActionResult> GetTopicRequest([FromQuery] TopicRequestParams requestParams)
    {
        var result = await groupService.GetTopicRequestsAsync(requestParams);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-available-topics")]
    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Manager}")]
    public async Task<IActionResult> GetAvailableTopicsForGroupAsync([FromQuery] TopicForGroupParams request)
    {
        var result = await topicService.GetAvailableTopicsForGroupAsync(request);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("information")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> GetGroupInformationAsync()
    {
        var result = await groupService.GetGroupInformationByGroupSelfId();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    #endregion

    #region ProjectProgress

    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Student}")]
    [HttpGet("{groupId}/progress")]
    public async Task<IActionResult> GetProjectProgressOfGroup(Guid groupId)
    {
        var result = await groupService.GetProjectProgressByGroup(groupId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpPost("progress/import")]
    public async Task<IActionResult> ImportProjectProgress([FromForm] ImportProjectProgressRequest request)
    {
        var result = await groupService.ImportProjectProgressFile(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpPut("progress")]
    public async Task<IActionResult> UpdateProjectProgress([FromBody] UpdateProjectProgressRequest request)
    {
        var result = await groupService.UpdateProjectProgress(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpPut("progress/week")]
    public async Task<IActionResult> UpdateProjectProgressWeek([FromBody] UpdateProjectProgressWeekRequest request)
    {
        var result = await groupService.UpdateProjectProgressWeek(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost("progress/tasks")]
    public async Task<IActionResult> CreateProjectProgressTask([FromBody] CreateTaskRequest request)
    {
        var result = await groupService.CreateTask(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Supervisor}")]
    [HttpGet("progress/{projectProgressId}/tasks")]
    public async Task<IActionResult> GetProjectProgressTasks(Guid projectProgressId)
    {
        var result = await groupService.GetTasks(projectProgressId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = $"{UserRoles.Student},{UserRoles.Supervisor}")]
    [HttpGet("progress/tasks/{taskId}")]
    public async Task<IActionResult> GetProjectProgressTaskDetail(Guid taskId)
    {
        var result = await groupService.GetTasksDetail(taskId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    [HttpGet("progress/{projectProgressId}/tasks/dashboard")]
    public async Task<IActionResult> DashBoardTaskOfGroup(Guid projectProgressId)
    {
        var result = await groupService.DashBoardTaskOfGroup(projectProgressId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPut("progress/tasks")]
    public async Task<IActionResult> UpdateProjectProgressTask([FromBody] UpdateTaskRequest request)
    {
        var result = await groupService.UpdateTask(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }


    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpPost("progress/week/evaluations")]
    public async Task<IActionResult> EvaluationsWeeklyProgress([FromBody] CreateWeeklyEvaluationRequest request)
    {
        var result = await groupService.CreateWeeklyEvaluations(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpGet("progress/week/evaluation/{groupId}")]
    public async Task<IActionResult> GetEvaluationWeeklyProgress(Guid groupId)
    {
        var result = await groupService.GetProgressEvaluationOfGroup(groupId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Student)]
    [HttpPost("progress/week/summary")]
    public async Task<IActionResult> CreateSummaryWeeklyProgress(SummaryProjectProgressWeekRequest request)
    {
        var result = await groupService.SummaryProjectProgressWeek(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpGet("progress/week/evaluation/{groupId}/excel")]
    public async Task<IActionResult> GetEvaluationWeeklyProgressFile(Guid groupId)
    {
        var result = await groupService.ExportProgressEvaluationOfGroup(groupId, default);

        return result.IsSuccess
            ? File(result.Value,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "Evaluation_Project_Progress.xlsx")
            : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpGet("manage")]
    public async Task<IActionResult> GetGroupManageBySupervisor()
    {
        var result = await groupService.GetGroupsWhichMentorBySupervisor(default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [Authorize(Roles = UserRoles.Supervisor)]
    [HttpGet("co-manage")]
    public async Task<IActionResult> GetGroupManageByCoSupervisor()
    {
        var result = await groupService.GetGroupsWhichMentorByCoSupervisor(default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    #endregion ProjectProgress

    #region GroupDocument

    [HttpPost("documents")]
    [Authorize(Roles = $"{UserRoles.Student}")]
    public async Task<IActionResult> UploadGroupDocument([FromForm] UploadGroupDocumentRequest request)
    {
        var result = await groupService.UploadGroupDocumentForGroup(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("documents/{groupId}")]
    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Manager},{UserRoles.Admin},{UserRoles.Student}")]
    public async Task<IActionResult> GetPresignGroupDocument(Guid groupId)
    {
        var result = await groupService.PresentGroupDocumentFileOfGroup(groupId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("group-decision")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupDecision([FromQuery] string? status)
    {
        var result = await groupService.GetGroupDecisionsByStatus(status);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    #endregion GroupDocument
}
