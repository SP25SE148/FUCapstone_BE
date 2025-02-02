using FluentValidation;
using FUC.Data.Entities;

namespace FUC.Service.DTOs.Validators;

public sealed class CampusValidator : AbstractValidator<Campus>
{
    public CampusValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Campus name must not be empty!!");
        RuleFor(c => c.Address)
            .NotEmpty()
            .WithMessage("Campus address must not be empty!!");
        RuleFor(c => c.Phone)
            .NotEmpty()
            .WithMessage("Campus phone must not be empty!!");
        RuleFor(c => c.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Campus email must not be empty!!");
    }
}
