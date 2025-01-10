using FluentValidation;

namespace FUC.Service.DTOs.Validators;
public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id is not empty");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name is not empty");

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage("Email is not correct");
    }
}
