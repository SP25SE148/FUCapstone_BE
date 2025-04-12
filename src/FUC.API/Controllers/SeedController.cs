using Microsoft.AspNetCore.Mvc;
using FUC.Common.Abstractions;
using MassTransit;
using FUC.Common.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using FUC.Data.Entities;
using System.Text.Json;
using FUC.API.Abstractions;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using FUC.Data.Data;

namespace FUC.API.Controllers;

public class SeedController : ApiController
{
    private readonly ILogger<SeedController> _logger;
    private readonly ICurrentUser _currentUser;
    private readonly IBus _bus;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IServiceProvider _serviceProvider;

    public SeedController(ILogger<SeedController> logger,
        ICurrentUser currentUser,
        IBus bus,
        IPublishEndpoint publishEndpoint,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _currentUser = currentUser;
        _bus = bus;
        _publishEndpoint = publishEndpoint;
        _serviceProvider = serviceProvider;
    }

    [HttpGet("test/users")]
    [Authorize]
    public async Task<IActionResult> TestUsers()
    {
        return Ok(new
        {
            _currentUser.Id,
            _currentUser.Name,
            _currentUser.Email,
            _currentUser.UserCode
        });
    }

    [HttpGet("topics")]
    public async Task<IActionResult> SeedTopics()
    {
        var topicsData = await System.IO.File.ReadAllTextAsync("SeedData/Topic.json");

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        options.Converters.Add(new JsonStringEnumConverter());

        var topics = JsonSerializer.Deserialize<List<Topic>>(topicsData, options);

        var context = _serviceProvider.GetRequiredService<FucDbContext>();

        context.DisableInterceptors = true;

        if (!context.Set<Topic>().Any() && topics is not null)
        {
            context.Set<Topic>().AddRange(topics);
        }

        await context.SaveChangesAsync();

        return Ok();
    }

    [HttpPost("test/bus")]
    public async Task<IActionResult> TestBus()
    {
        _logger.LogInformation("Test publish message into queue");

        var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/fuc-users-sync-message"));

        await endpoint.Send(new UsersSyncMessage
        {
            AttempTime = 1,
            CreatedBy = "abc",
            UsersSync = [],
            UserType = "abc"
        });

        return Ok();
    }

    [HttpPost("test/pub")]
    public async Task<IActionResult> TestPub()
    {
        _logger.LogInformation("Test publish message into queue");

        var endpoint = await _bus.GetSendEndpoint(new Uri("rabbitmq://localhost/fuc-users-sync-message"));

        await endpoint.Send(new UsersSyncMessage
        {
            AttempTime = 1,
            CreatedBy = "abc",
            UsersSync = [],
            UserType = "abc"
        });

        return Ok();
    }
}
