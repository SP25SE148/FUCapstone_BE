using FUC.API.Abstractions;
using FUC.Common.Abstractions;
using FUC.Common.Constants;
using FUC.Common.Shared;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.DefendCapstone;
using FUC.Service.DTOs.GroupDTO;
using FUC.Service.DTOs.ReviewCalendarDTO;
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
    IReviewCalendarService reviewCalendarService,
    IDefendCapstoneService defendCapstoneService,
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

    [HttpGet("get-remain-students")]
    [Authorize(Roles = $"{UserRoles.SuperAdmin}, {UserRoles.Admin}, {UserRoles.Manager}")]
    public async Task<IActionResult> GetRemainStudenstAsync()
    {
        var result = await studentService.GetRemainStudentsAsync(default);

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
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> ImportReviewAsync(IFormFile file)
    {
        var result = await reviewCalendarService.ImportReviewCalendar(file);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("student/get-review-calendar")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> GetReviewCalendarByStudentAsync()
    {
        var result = await reviewCalendarService.GetReviewCalendarByStudentId();

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("supervisor/get-review-calendar")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> GetReviewCalendarBySupervisorAsync()
    {
        var result = await reviewCalendarService.GetReviewCalendarBySupervisorId();

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("manager/get-review-calendar")]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<IActionResult> GetReviewCalendarByManagerAsync()
    {
        var result = await reviewCalendarService.GetReviewCalendarByManagerId();

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("defend/calendar")]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<IActionResult> ImportDefendCapstoneCalendarAsync([FromForm] IFormFile file)
    {
        var result = await defendCapstoneService.UploadDefendCapstoneProjectCalendar(file, default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut("supervisor/update-group-decision-status")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> UpdateGroupDecisionStatusBySupervisorAsync(
        [FromBody] UpdateGroupDecisionStatusBySupervisorRequest request)
    {
        var result = await groupService.UpdateGroupDecisionBySupervisorIdAsync(request);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut("defend/president-decision-status")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> UpdateGroupDecisionStatusByPresidentAsync(
        [FromBody] UpdateGroupDecisionStatusByPresidentRequest request)
    {
        var result = await defendCapstoneService.UpdateStatusOfGroupAfterDefend(request, default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("defend/calendar")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetDefendCalendarAsync()
    {
        var result = await defendCapstoneService.GetDefendCalendersByCouncilMember(default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("defend/calendar/manager")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> GetDefendCalendarByManagerAsync()
    {
        var result = await defendCapstoneService.GetDefendCalendersByManager(default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("defend/thesis")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> UploadThesisCouncilMeetingMinutesForDefendCapstone(
        [FromForm] UploadThesisCouncilMeetingMinutesRequest request)
    {
        var result = await defendCapstoneService
            .UploadThesisCouncilMeetingMinutesForDefendCapstone(request, default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("defend/thesis/{calendarId}")]
    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Manager}")]
    public async Task<IActionResult> PresentThesisForTopicResignedUrl(Guid calendarId)
    {
        var result = await defendCapstoneService.PresentThesisForTopicResignedUrl(calendarId, default);

        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPut("supervisor/update-reviewer-suggestion-and-comment")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> UpdateReviewerSuggestionAndCommentAsync(
        [FromBody] UpdateReviewerSuggestionAndCommentRequest request)
    {
        var result = await reviewCalendarService.UpdateReviewCalendar(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("review-calendar-result/student")]
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> GetReviewCalendarResultByStudentAsync()
    {
        var result = await reviewCalendarService.GetReviewCalendarResultByStudentId();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("review-calendar-result/manager")]
    [Authorize(Roles = UserRoles.Manager)]
    public async Task<IActionResult> GetReviewCalendarResultByManagerAsync()
    {
        var result = await reviewCalendarService.GetReviewCalendarResultByManagerId();
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("review-calendar-result/{groupId}")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> GetReviewCalendarResultBySupervisorAsync(Guid groupId)
    {
        var result = await reviewCalendarService.GetReviewCalendarResultByGroupId(groupId);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("review-criteria/{attempt}")]
    public async Task<IActionResult> GetReviewCriteriaAsync(int attempt)
    {
        var result = await reviewCalendarService.GetReviewCriteriaByAttemptAsync(attempt);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("defend/calendar/{id}")]
    [Authorize(Roles = UserRoles.Supervisor)]
    public async Task<IActionResult> GetDefendCalendarByIdAsync(Guid id)
    {
        var result = await defendCapstoneService.GetDefendCapstoneCalendarByIdAsync(id);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }
}
