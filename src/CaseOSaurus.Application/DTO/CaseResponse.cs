using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Application.DTO;

public record CaseResponse(
    Guid Id,
    string Title,
    string? Description,
    Priority Priority,
    CaseType Type,
    string Requestor,
    string? AssignedTo,
    CaseStatus Status,
    DateTime CreatedAt,
    string CreatedBy
);
