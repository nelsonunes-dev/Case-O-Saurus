using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Enums;

namespace CaseOSaurus.Application.Commands.UpdateCaseStatus;

public record UpdateCaseStatusCommand(Guid CaseId, CaseStatus NewStatus) : ICommand<CaseResponse>;
