using System.Diagnostics;
using FUC.Common.Constants;
using FUC.Common.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Identity.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    protected IActionResult HandleFailure(OperationResult result) =>
        result switch
        {
            { IsSuccess: true } => throw new InvalidOperationException(),
            IValidationResult validationResult =>
                BadRequest(
                    CreateProblemDetails(
                        result.Error,
                        validationResult.Errors)),
            _ =>
                BadRequest(
                    CreateProblemDetails(
                        result.Error))
        };

    private static ProblemDetails CreateProblemDetails(
        Error error,
        Error[]? errors = null) =>
        new()
        {
            Title = error.Code,
            Type = Constants.BadRequestType,
            Detail = error.Message,
            Status = StatusCodes.Status400BadRequest,
            Extensions = {
                {
                    nameof(errors),
                    errors?.GroupBy(e => e.Code)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Select(e => e.Message)
                        )
                },
                {
                    "traceId",
                    Activity.Current?.Id
                }
            }
        };
}
