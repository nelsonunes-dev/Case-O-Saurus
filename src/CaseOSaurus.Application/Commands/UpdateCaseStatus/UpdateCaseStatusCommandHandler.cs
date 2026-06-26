using CaseOSaurus.Application.Common.Exceptions;
using CaseOSaurus.Application.Common.Interfaces;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CaseOSaurus.Application.Commands.UpdateCaseStatus;

public class UpdateCaseStatusCommandHandler : ICommandHandler<UpdateCaseStatusCommand, CaseResponse>
{
    private readonly IApplicationDbContext _context;

    public UpdateCaseStatusCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<CaseResponse> Handle(UpdateCaseStatusCommand command, CancellationToken cancellationToken)
    {
        var caseEntity = await _context.Cases.FirstOrDefaultAsync(c => c.Id == command.CaseId, cancellationToken);

        if (caseEntity is null)
            throw new NotFoundException(nameof(UserCase), command.CaseId);

        // Validate state transition (domain method will throw if invalid)
        try
        {
            caseEntity.ChangeStatus(command.NewStatus);
        }
        catch (InvalidOperationException ex)
        {
            // Wrap in a business rule exception or just rethrow
            throw new BusinessRuleException(ex.Message);
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConcurrencyException("The case was modified by another user. Please reload and try again.");
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
