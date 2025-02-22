using FUC.API.Abstractions;
using FUC.Common.Constants;
using FUC.Service.Abstractions;
using FUC.Service.DTOs.TopicDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FUC.API.Controllers;

public class TopicsController(ITopicService topicService) : ApiController
{
    [HttpPost]
    [Authorize(Roles = $"{UserRoles.Supervisor}")]
    public async Task<IActionResult> CreateTopic([FromForm] CreateTopicRequest request)
    {
        var result = await topicService.CreateTopic(request, cancellationToken: default);

        return result.IsSuccess ? 
            Ok(result) :
            HandleFailure(result);
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
