using Amazon.S3.Model;
using Amazon.S3;
using FUC.API.Abstractions;
using FUC.Common.Shared;
using FUC.Service.DTOs;
using Microsoft.AspNetCore.Mvc;
using FUC.Service.Abstractions;
using FUC.Service.Extensions.Options;
using Microsoft.Extensions.Options;

namespace FUC.API.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ApiController
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly IS3Service _s3Service;
    private readonly S3Settings s3Settings;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, IS3Service s3Service, IOptions<S3Settings> options)
    {
        _logger = logger;
        _s3Service = s3Service;
        s3Settings = options.Value; 
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        _logger.LogInformation("Get weather");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }

    [HttpGet("images/{key}")]
    public async Task<IActionResult> GetImage()
    {
        try
        {
            var response = await _s3Service.GetFromS3(s3Settings.Bucket, "images/22bc2fb5-4e8f-4aef-aa87-db9dce5d8216");

            return response is not null ? File(response.ResponseStream, response.Headers.ContentType, response.Metadata["file-name"]) : BadRequest("Not found");
        }
        catch (AmazonS3Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
