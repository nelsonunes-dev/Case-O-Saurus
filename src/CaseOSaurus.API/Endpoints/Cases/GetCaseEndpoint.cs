using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.Queries.GetCases;
using CaseOSaurus.Domain.Enums;
using FastEndpoints;
using Microsoft.AspNetCore.Authorization;

namespace CaseOSaurus.API.Endpoints.Cases;

[AllowAnonymous]
public class GetCaseEndpoint : EndpointWithoutRequest<List<CaseSummary>>
{
    private readonly IMediator _mediator;

    public GetCaseEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("/api/cases");
        AllowAnonymous(); // Will be secured later
        Summary(s =>
        {
            s.Summary = "Get all cases";
            s.Description = "Returns a list of all cases, optionally filtered by status.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var statusParam = Query<string>("status", isRequired: false);
        CaseStatus? status = null;
        if (!string.IsNullOrEmpty(statusParam) && Enum.TryParse<CaseStatus>(statusParam, true, out var parsedStatus))
        {
            status = parsedStatus;
        }

        var query = new GetCasesQuery(status);
        var result = await _mediator.Send(query, ct);

        await Send.OkAsync(result, ct);
    }
}
