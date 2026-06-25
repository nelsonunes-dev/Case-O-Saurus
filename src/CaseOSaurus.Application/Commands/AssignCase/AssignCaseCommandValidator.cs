using FluentValidation;

namespace CaseOSaurus.Application.Commands.AssignCase;

public class AssignCaseCommandValidator : AbstractValidator<AssignCaseCommand>
{
    public AssignCaseCommandValidator()
    {
        RuleFor(x => x.CaseId)
            .NotEmpty().WithMessage("Case ID is required.");

        RuleFor(x => x.AssignedTo)
            .NotEmpty().WithMessage("AssignedTo is required.")
            .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("AssignedTo must be a valid user ID format (alphanumeric or hyphens).");
    }
}
