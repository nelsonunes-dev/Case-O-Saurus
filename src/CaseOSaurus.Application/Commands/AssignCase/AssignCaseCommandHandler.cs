using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Application.Common.Interfaces;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseOSaurus.Application.Commands.AssignCase;

public class AssignCaseCommandHandler : ICommandHandler<AssignCaseCommand, CaseResponse>
{
    private readonly IApplicationDbContext _context;

    public AssignCaseCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CaseResponse> Handle(AssignCaseCommand command, CancellationToken cancellationToken)
    {
        var caseEntity = await _context.Cases.FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);

        if (caseEntity is null)
            throw new NotFoundException(nameof(UserCase), command.CaseId);

        // Update the entity using domain method
        caseEntity.AssignTo(command.AssignedTo);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException($"The case was modified by another user. Please reload and try again.");
        }

        return new CaseResponse(
            caseEntity.Id,
            caseEntity.Title,
            caseEntity.Description,
            caseEntity.Priority,
            caseEntity.Type,
            caseEntity.Requestor,
            caseEntity.AssignedTo,
            caseEntity.Status,
            caseEntity.CreatedAt,
            caseEntity.CreatedBy
        );
    }
}
