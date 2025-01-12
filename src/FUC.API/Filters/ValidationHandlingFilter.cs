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

            if (!result.IsValid)
            {
                context.Result = new BadRequestObjectResult(HandleValidationException(result.ToDictionary()));
                return;
                
            }
        }

        await next();
    }

    private static ValidationProblemDetails HandleValidationException(IDictionary<string, string[]> errors)
    {
        var problemDetails = new ValidationProblemDetails
        {
            Title = IValidationResult.ValidationError.Code,
            Type = Constants.BadRequestType,
            Status = StatusCodes.Status400BadRequest,
            Errors = errors,
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
