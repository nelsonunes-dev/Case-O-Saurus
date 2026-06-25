using FluentValidation;

namespace CaseOSaurus.Application.Commands.CreateCase;

public class CreateCaseCommandValidator : AbstractValidator<CreateCaseCommand>
{
    public CreateCaseCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Requestor)
            .NotEmpty().WithMessage("Requestor is required.")
            .EmailAddress().WithMessage("Requestor must be a valid email address.");

        RuleFor(x => x.Priority)
            .IsInEnum().WithMessage("Invalid priority value.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid case type value.");

        When(x => !string.IsNullOrEmpty(x.AssignedTo), () =>
        {
            RuleFor(x => x.AssignedTo)
                .Matches(@"^[a-zA-Z0-9\-]+$").WithMessage("AssignedTo must be a valid user ID format.");
        });
    }
}
