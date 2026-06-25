using CaseOSaurus.Application.Commands.AssignCase;
using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Application.CQRS.Interfaces;
using FastEndpoints;
using FluentValidation;

namespace CaseOSaurus.API.Endpoints.Cases;

public class AssignCaseRequest
{
    public string AssignedTo { get; set; } = string.Empty;
}

public class AssignCaseEndpoint : Endpoint<AssignCaseRequest>
{
    private readonly IMediator _mediator;
    private readonly IValidator<AssignCaseCommand> _validator;

    public AssignCaseEndpoint(IMediator mediator, IValidator<AssignCaseCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public override void Configure()
    {
        Patch("/api/cases/{id}/assign");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Assign a case to a user";
            s.Description = "Updates the AssignedTo field of an existing case.";
        });
    }

    public override async Task HandleAsync(AssignCaseRequest req, CancellationToken ct)
    {
        var caseId = Route<Guid>("id");

        var command = new AssignCaseCommand(caseId, req.AssignedTo);
        var validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
                AddError(error.ErrorMessage, error.PropertyName);
            
            await Send.ErrorsAsync(400, ct);
            return;
        }

        try
        {
            var result = await _mediator.Send(command, ct);

            var response = new
            {
                result.Id,
                result.Title,
                result.Description,
                Priority = result.Priority.ToString(),
                Type = result.Type.ToString(),
                result.Requestor,
                result.AssignedTo,
                Status = result.Status.ToString(),
                result.CreatedAt,
                result.CreatedBy
            };

            await Send.OkAsync(response, ct);
        }
        catch (NotFoundException)
        {
            await Send.NotFoundAsync(ct);
        }
        catch (ConcurrencyException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(409, ct);
        }
    }
}
