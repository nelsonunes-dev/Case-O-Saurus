using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Application.Commands.CreateCase;

public record CreateCaseCommand(
    string Title,
    string? Description,
    Priority Priority,
    CaseType Type,
    string Requestor,
    string? AssignedTo = null
) : ICommand<CaseResponse>;
