using FUC.Common.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;
using FUC.Common.Constants;

namespace FUC.API.Filters;

public sealed class ValidationHandlingFilter(IServiceProvider serviceProvider) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var parameter = context.ActionDescriptor
            .Parameters
            .FirstOrDefault(p => p.BindingInfo?.BindingSource == BindingSource.Body);

        if (parameter != null &&
            serviceProvider.GetService(typeof(IValidator<>).MakeGenericType(parameter!.ParameterType)) is IValidator validator)
        {
            var subject = context.ActionArguments[parameter.Name];
            var result = await validator.ValidateAsync(new ValidationContext<object>(subject!), context.HttpContext.RequestAborted);

            Error[] errors = result.Errors
                .Where(e => e is not null)
                .Select(failure => new Error(
                    failure.PropertyName,
                    failure.ErrorMessage
                ))
                .Distinct()
                .ToArray();

            if (errors.Length > 0)
            {
                context.Result = new BadRequestObjectResult(HandleValidationException(errors));
                return;
            }
        }

        await next();
    }

    private static ValidationProblemDetails HandleValidationException(Error[] errors)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = IValidationResult.ValidationError.Code,
            Type = Constants.BadRequestType,
            Status = StatusCodes.Status400BadRequest,
            Errors = errors.GroupBy(e => e.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(e => e.Message).ToArray()
                ) ?? throw new InvalidOperationException(),
            Extensions = new Dictionary<string, object?>
            {
                {
                    "traceId",
                    Activity.Current?.Id
                }
            }
        };

        return problemDetails;
    }
}
