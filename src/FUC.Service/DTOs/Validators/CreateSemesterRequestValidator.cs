using FluentValidation;
using FUC.Service.DTOs.SemesterDTO;

namespace FUC.Service.DTOs.Validators;

public class CreateSemesterRequestValidator : AbstractValidator<CreateSemesterRequest>
{
    public CreateSemesterRequestValidator()
    {
        RuleFor(x => x.StartDate)
            .NotNull()
            .LessThan(x => x.EndDate)
            .WithMessage("Start date must be before end date");

        RuleFor(x => x.EndDate)
            .NotNull()
            .Must((request, endDate) => (endDate - request.StartDate).Days >= 90)
            .WithMessage("End date must be at least 3 months after start date");
        RuleFor(x => x.Id)
            .NotNull()
            .WithMessage("Id of semester must not be empty");
    }
}
