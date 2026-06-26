using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Application.Queries.GetCases;

public record GetCasesQuery(CaseStatus? Status = null) : IQuery<List<CaseSummary>>;
