using CaseOSaurus.Application.Commands.CreateCase;
using CaseOSaurus.Application.CQRS.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace CaseOSaurus.API.Endpoints.Cases;

public class CreateCaseRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Requestor { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
}

public class CreateCaseResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Priority { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Requestor { get; set; } = string.Empty;
    public string? AssignedTo { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
}

public class CreateCaseEndpoint : Endpoint<CreateCaseRequest, CreateCaseResponse>
{
    private readonly IMediator _mediator;
    private readonly IValidator<CreateCaseCommand> _validator;

    public CreateCaseEndpoint(IMediator mediator, IValidator<CreateCaseCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public override void Configure()
    {
        Post("/api/cases");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Create a new case";
            s.Description = "Creates a new case with the provided details.";
        });
    }

    public override async Task HandleAsync(CreateCaseRequest req, CancellationToken ct)
    {
        if (!Enum.TryParse<Domain.Enums.Priority>(req.Priority, true, out var priority))
            AddError("Invalid Priority value. Allowed: Low, Medium, High");

        if (!Enum.TryParse<Domain.Enums.CaseType>(req.Type, true, out var caseType))
            AddError("Invalid Type value. Allowed: ServiceRequest, Incident, Complaint");

        var command = new CreateCaseCommand(
            req.Title,
            req.Description,
            priority,
            caseType,
            req.Requestor,
            req.AssignedTo
        );

        var validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                AddError(error.ErrorMessage, error.PropertyName);
        }

        if (ValidationFailed)
        {
            await Send.ErrorsAsync();
            return;
        }

        var result = await _mediator.Send(command, ct);

        var response = new CreateCaseResponse
        {
            Id = result.Id,
            Title = result.Title,
            Description = result.Description,
            Priority = result.Priority.ToString(),
            Type = result.Type.ToString(),
            Requestor = result.Requestor,
            AssignedTo = result.AssignedTo,
            Status = result.Status.ToString(),
            CreatedAt = result.CreatedAt,
            CreatedBy = result.CreatedBy
        };

        await Send.OkAsync(response, ct);
    }
}
