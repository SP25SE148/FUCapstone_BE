using Amazon.Runtime.Internal.Transform;
using FUC.API.Abstractions;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.StudentDTO;
using FUC.Service.DTOs.SupervisorDTO;
using FUC.Service.DTOs.TopicRequestDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

[Authorize]
public sealed class UserController(
    ICurrentUser currentUser,
    IStudentService studentService,
    ISupervisorService supervisorService,
    IGroupService groupService) : ApiController
{
    [HttpGet("get-all-student")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}, {UserRoles.Admin}, {UserRoles.Manager}")]
    public async Task<IActionResult> GetAllStudentAsync()
    {
        OperationResult<IEnumerable<StudentResponseDTO>> result = await studentService.GetAllStudentAsync(default);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("get-all-supervisor")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}, {UserRoles.Admin}, {UserRoles.Manager}")]
    public async Task<IActionResult> GetAllSupervisorAsync()
    {
        OperationResult<IEnumerable<SupervisorResponseDTO>> result =
            await supervisorService.GetAllSupervisorAsync(default);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("student/{id}")]
    public async Task<IActionResult> GetStudentInformation(string id)
    {
        OperationResult<StudentResponseDTO> studentInfo = await studentService.GetStudentByIdAsync(id);
        return studentInfo.IsSuccess
            ? Ok(studentInfo)
            : HandleFailure(studentInfo);
    }

    [HttpPut("student")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> UpdateStudentInformation([FromBody] UpdateStudentRequest request)
    {
        var result = await studentService.UpdateStudentInformation(request, currentUser.UserCode);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut("update-topic-request-status")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> UpdateTopicRequestAsync([FromBody] UpdateTopicRequestStatusRequest request)
    {
        var result = await groupService.UpdateTopicRequestStatusAsync(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("import-review")]
    public async Task<IActionResult> ImportReviewAsync(IFormFile file)
    {
        var result = await groupService.ImportReviewCalendar(file);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }
}
