using Amazon.S3;
using FUC.API.Abstractions;
using Microsoft.AspNetCore.Mvc;
using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;
using FUC.Common.Abstractions;
using MassTransit;
using FUC.Common.Contracts;

namespace FUC.API.Controllers;

public class WeatherForecastController : ApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IS3Service _s3Service;
    private readonly S3Settings s3Settings;
    private readonly ICurrentUser _currentUser;
    private readonly IBus _bus;
    private readonly IPublishEndpoint _publishEndpoint;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, 
        IS3Service s3Service, 
        IOptions<S3Settings> options,
        ICurrentUser currentUser,
        IBus bus,
        IPublishEndpoint publishEndpoint)
    {
        _logger = logger;
        _s3Service = s3Service;
        s3Settings = options.Value; 
        _currentUser = currentUser;
        _bus = bus;
        _publishEndpoint = publishEndpoint; 
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize(Roles = "Student")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation(_currentUser.Id);
        _logger.LogInformation(_currentUser.Name);
        _logger.LogInformation(_currentUser.Email);
        _logger.LogInformation(_currentUser.UserCode);
        _logger.LogInformation("Get weather");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
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

    [HttpGet("images/{key}")]
    public async Task<IActionResult> GetImage(string key)
    {
        try
        {
            var response = await _s3Service.GetFromS3(s3Settings.Bucket, $"images/{key}");

            return response is not null ? File(response.ResponseStream, response.Headers.ContentType, response.Metadata["file-name"]) : BadRequest("Not found");
        }
        catch (AmazonS3Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
