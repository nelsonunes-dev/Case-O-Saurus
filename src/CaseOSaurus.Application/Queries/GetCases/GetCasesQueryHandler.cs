using CaseOSaurus.Application.Common.Interfaces;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseOSaurus.Application.Queries.GetCases;

public class GetCasesQueryHandler : IQueryHandler<GetCasesQuery, List<CaseSummary>>
{
    private readonly IApplicationDbContext _context;

    public GetCasesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CaseSummary>> Handle(GetCasesQuery query, CancellationToken cancellationToken)
    {
        var casesQuery = _context.Cases.AsQueryable();

        if (query.Status.HasValue)
            casesQuery = casesQuery.Where(c => c.Status == query.Status.Value);

        var cases = await casesQuery
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new CaseSummary(
                c.Id,
                c.Title,
                c.Status.ToString(),
                c.AssignedTo,
                c.CreatedAt
            ))
            .ToListAsync(cancellationToken);

        return cases;
    }
}
