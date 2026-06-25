using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;

namespace CaseOSaurus.Application.Commands.AssignCase;

public record AssignCaseCommand(Guid CaseId, string AssignedTo) : ICommand<CaseResponse>;