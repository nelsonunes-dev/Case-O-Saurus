using FluentValidation;

namespace CaseOSaurus.Application.Commands.UpdateCaseStatus;

public class UpdateCaseStatusCommandValidator : AbstractValidator<UpdateCaseStatusCommand>
{
    public UpdateCaseStatusCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .NotEmpty().WithMessage("Case ID is required.");

        RuleFor(x => x.NewStatus)
            .IsInEnum().WithMessage("Invalid status value.");
    }
}
