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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

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

        var context = _serviceProvider.GetService<DbContext>();

        var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

        // Manually create a fake HttpContext with a test user
        var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "vulns"),
                new Claim("name", "Le Nguyen Son Vu"),
                new Claim(ClaimTypes.Email, "vulns@fe.edu.vn"),
                new Claim(ClaimTypes.GivenName, "vulns")
            };

        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);

        // Set the fake user in IHttpContextAccessor
        httpContextAccessor.HttpContext = new DefaultHttpContext { User = user };

        if(!context.Set<Topic>().Any() && topics is not null)
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
