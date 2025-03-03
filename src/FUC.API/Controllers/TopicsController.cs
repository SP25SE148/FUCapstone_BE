using FUC.API.Abstractions;
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
    public async Task<IActionResult> GetTopics([FromQuery] TopicRequest request)
    {
        var result = await topicService.GetTopics(request);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("supervisor")]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> GetTopicsBySupervisor()
    {
        var result = await topicService.GetTopicsBySupervisor();

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
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> SemanticTopicReviewer(string id)
    {
        var result = await topicService.SemanticTopic(Guid.Parse(id), withCurrentSemester: true, default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost("assign-topic-appraisal")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> AssignTopicAppraisal([FromBody] IReadOnlyList<string> supervisorEmail)
    {
        var result = await topicService.CreateTopicAppraisal(supervisorEmail);
        return result.IsSuccess ? Ok() : HandleFailure(result);
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

    [HttpPost("appraisal/final")]
    [Authorize(Roles = $"{UserRoles.Manager}")]
    public async Task<IActionResult> SubmitFinalAppraisalTopic([FromBody] FinalAppraisalTopicRequest request)
    {
        var result = await topicService.FinalSubmitAppraisalTopic(request, default);
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
}
