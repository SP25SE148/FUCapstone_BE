﻿using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.TopicAppraisalDTO;
using FUC.Service.DTOs.TopicDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public class TopicsController(ITopicService topicService) : ApiController
{
    [HttpGet("{topicId}")]
    public async Task<IActionResult> GetTopicById(Guid topicId)
    {
        var result = await topicService.GetTopicById(topicId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetTopics([FromQuery] TopicParams requestParams)
    {
        var result = await topicService.GetTopics(requestParams);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("supervisor")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetTopicsBySupervisor()
    {
        var result = await topicService.GetTopicsBySupervisor();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("manager")]
    [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Admin}")]
    public async Task<IActionResult> GetTopicsByManagerLevel()
    {
        var result = await topicService.GetTopicsByManagerLevel();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("presigned/{topicId}")]
    public async Task<IActionResult> GetPresignedUrlTopic(string topicId)
    {
        var result = await topicService.PresentTopicPresignedUrl(Guid.Parse(topicId), default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> CreateTopic([FromForm] CreateTopicRequest request)
    {
        var result = await topicService.CreateTopic(request, cancellationToken: default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("{topicId}")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> UpdateTopic(Guid topicId, [FromForm] UpdateTopicRequest request)
    {
        request.TopicId = topicId;
        var result = await topicService.UpdateTopic(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("statistic/{topId}")]
    [Authorize(Roles = $"{UserRoles.Supervisor},{UserRoles.Manager}")]
    public async Task<IActionResult> GetStatisticTopics(string topId)
    {
        var result = await topicService.GetTopicAnalysises(Guid.Parse(topId), default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("semantic/{id}")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> SemanticTopic(string id)
    {
        var result = await topicService.SemanticTopic(Guid.Parse(id), withCurrentSemester: false, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("semantic/appraisal/{id}")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> SemanticTopicReviewer(string id)
    {
        var result = await topicService.SemanticTopic(Guid.Parse(id), withCurrentSemester: true, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("assign-topic-appraisal")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> AssignTopicAppraisal([FromBody] IReadOnlyList<string> supervisorEmail)
    {
        var result = await topicService.AssignTopicAppraisalForAvailableSupervisors(supervisorEmail, default);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("assign-topic-appraisal/supervisor")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> AssignTopicAppraisalForSupervisor([FromBody] AssignSupervisorAppraisalTopicRequest request)
    {
        var result = await topicService.AssignSupervisorForAppraisalTopic(request, default);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("remove-topic-appraisal")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> RemoveAssignTopicAppraisalForSupervisor(
        [FromBody] RemoveAssignSupervisorAppraisalTopicRequest request)
    {
        var result = await topicService.RemoveAssignSupervisorForAppraisalTopic(request, default);
        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("get-topic-appraisal-by-self")]
    [Authorize(Roles = $"{UserRoles.Manager},{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetTopicAppraisals([FromQuery] TopicAppraisalBaseRequest request)
    {
        var result = await topicService.GetTopicAppraisalByUserId(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }


    [HttpGet("get-supervisor-topic-appraisal")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> GetTopicAppraisalsBySupervisorId([FromQuery] ManagerTopicAppraisalRequest request)
    {
        var result = await topicService.GetTopicAppraisalByUserId(request);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpPost("appraisal")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> SubmitAppraisalTopic([FromBody] AppraisalTopicRequest request)
    {
        var result = await topicService.AppraisalTopic(request, default);
        return result.IsSuccess
            ? Ok(result)
            : HandleFailure(result);
    }

    [HttpGet("business")]
    public async Task<IActionResult> GetAllBusinessAreas()
    {
        var result = await topicService.GetAllBusinessAreas();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPut("assign/supervisor")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> AssignNewSupervisorForTopic([FromBody] AssignNewSupervisorForTopicRequest request)
    {
        var result = await topicService.AssignNewSupervisorForTopic(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("cosupervisor")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetTopicsByCoSupervisor()
    {
        var result = await topicService.GetTopicsByCoSupervisor();

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("cosupervisor")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> AddCoSupervisorForTopic([FromBody] AssignNewSupervisorForTopicRequest request)
    {
        var result = await topicService.AddCoSupervisorForTopic(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpDelete("cosupervisor")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> RemoveCoSupervisorForTopic([FromBody] RemoveCoSupervisorForTopicRequest request)
    {
        var result = await topicService.RemoveCoSupervisorForTopic(request, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("re-appraisal/{topicId}")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> ReAppraisalTopicForMainSupervisorOfTopic(Guid topicId)
    {
        var result = await topicService.ReAppraisalTopicForMainSupervisorOfTopic(topicId, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }
}
