using CaseOSaurus.Application.Commands.UpdateCaseStatus;
using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Domain.Enums;
using FastEndpoints;
using FluentValidation;

namespace CaseOSaurus.API.Endpoints.Cases;

public class UpdateCaseStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateCaseStatusEndpoint : Endpoint<UpdateCaseStatusRequest>
{
    private readonly IMediator _mediator;
    private readonly IValidator<UpdateCaseStatusCommand> _validator;

    public UpdateCaseStatusEndpoint(IMediator mediator, IValidator<UpdateCaseStatusCommand> validator)
    {
        _mediator = mediator;
        _validator = validator;
    }

    public override void Configure()
    {
        Patch("/api/cases/{id}/status");
        AllowAnonymous(); // Will be secured later
        Summary(s =>
        {
            s.Summary = "Update case status";
            s.Description = "Changes the status of a case (Open → InProgress → Closed).";
        });
    }

    public override async Task HandleAsync(UpdateCaseStatusRequest req, CancellationToken ct)
    {
        var caseId = Route<Guid>("id");

        if (!Enum.TryParse<CaseStatus>(req.Status, true, out var newStatus))
        {
            AddError("Invalid status value. Allowed: Open, InProgress, Closed.");
            await Send.ErrorsAsync(400, ct);
            return;
        }

        var command = new UpdateCaseStatusCommand(caseId, newStatus);

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
        catch (BusinessRuleException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(400, ct);
        }
        catch (ConcurrencyException ex)
        {
            AddError(ex.Message);
            await Send.ErrorsAsync(409, ct);
        }
    }
}
