using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.TopicDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public class TopicsController(ITopicService topicService) : ApiController
{
    [HttpGet]
    public async Task<IActionResult> GetTopics([FromQuery] TopicRequest request)
    {
        var result = await topicService.GetTopics(request);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> CreateTopic([FromForm] CreateTopicRequest request)
    {
        var result = await topicService.CreateTopic(request, cancellationToken: default);

        return result.IsSuccess ? 
            Ok(result) :
            HandleFailure(result);
    }

    [HttpGet("statistic/{topId}")]
    public async Task<IActionResult> GetStatisticTopics(string topId)
    {
        var result = await topicService.GetTopicAnalysises(Guid.Parse(topId), default);

        return result.IsSuccess ? Ok(result) : HandleFailure(result);
    }

    [HttpGet("business")]
    public async Task<IActionResult> GetAllBusinessAreas() 
    {
        var result = await topicService.GetAllBusinessAreas();

        return result.IsSuccess ?
            Ok(result) :
            HandleFailure(result);
    } 
}
