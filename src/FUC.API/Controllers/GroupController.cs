using FUC.API.Abstractions;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.IntegrationEventLog.Services;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.GroupMemberDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public class GroupController(
    IGroupService groupService,
    IGroupMemberService groupMemberService,
    ICurrentUser currentUser,
    IIntegrationEventLogService integrationEventLogService) : ApiController
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

    [HttpPut("{groupId}")]
    [Authorize(Roles = nameof(UserRoles.Student))]
    public async Task<IActionResult> CreateGroupCodeAsync()
    {
        var result = await groupService.UpdateGroupStatusAsync();


        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-student-id")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> GetGroupInfoByStudentId()
    {
        var result = await groupService.GetGroupByStudentIdAsync();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public async Task<IActionResult> GetGroups()
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupAsync();
        return !result.IsFailure
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-semester-id/{semesterId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsBySemesterId(string semesterId)
    {
        OperationResult<IEnumerable<GroupResponse>>
            result = await groupService.GetAllGroupBySemesterIdAsync(semesterId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-major-id/{majorId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsByMajorId(string majorId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupByMajorIdAsync(majorId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-capstone-id/{capstoneId}")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin},{UserRoles.Admin},{UserRoles.Manager}")]
    public async Task<IActionResult> GetGroupsByCampusId(string capstoneId)
    {
        OperationResult<IEnumerable<GroupResponse>>
            result = await groupService.GetAllGroupByCapstoneIdAsync(capstoneId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetGroupByIdAsync(Guid id)
    {
        OperationResult<GroupResponse> result = await groupService.GetGroupByIdAsync(id);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-by-campus-id/{campusId}")]
    [Authorize(Roles = UserRoles.SuperAdmin)]
    public async Task<IActionResult> GetGroupByCampusIdAsync(string campusId)
    {
        OperationResult<IEnumerable<GroupResponse>> result = await groupService.GetAllGroupByCampusIdAsync(campusId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    #endregion

    #region Group Member Endpoint

    [Authorize(Roles = nameof(UserRoles.Student))]
    [HttpPost("add-member")]
    public async Task<IActionResult> AddMemberIntoGroupAsync(CreateGroupMemberRequest request)
    {
        OperationResult<Guid> result = await groupMemberService.CreateBulkGroupMemberAsync(request);
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
}
