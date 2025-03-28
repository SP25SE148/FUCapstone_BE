using FluentValidation;
using FluentValidation.Results;
using FUC.Data.Enums;
using FUC.Service.DTOs.GroupDTO;

namespace FUC.Service.DTOs.Validators;

public class UpdateGroupDecisionStatusValidator : AbstractValidator<UpdateGroupDecisionStatusBySupervisorRequest>
{
    public UpdateGroupDecisionStatusValidator()
    {
        RuleFor(x => x.GroupId).NotEmpty().WithMessage("group id is not empty");
        RuleFor(x => x.DecisionStatus)
            .NotEmpty()
            .WithMessage("decision status can not be empty");
    }
}
