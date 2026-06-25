using AutoMapper;
using CaseOSaurus.Application.Common.Interfaces;
using CaseOSaurus.Application.Common.Services;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Entities;

namespace CaseOSaurus.Application.Commands.CreateCase;

public class CreateCaseCommandHandler : ICommandHandler<CreateCaseCommand, CaseResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public CreateCaseCommandHandler(IApplicationDbContext context, IMapper mapper, IUserContext userContext)
    {
        _context = context;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<CaseResponse> Handle(CreateCaseCommand command, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId ?? "system"; // fallback if not authenticated

        var newCase = new UserCase(
            title: command.Title,
            description: command.Description,
            priority: command.Priority,
            type: command.Type,
            requestor: command.Requestor,
            createdBy: userId,
            assignedTo: command.AssignedTo
        );

        _context.Cases.Add(newCase);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CaseResponse>(newCase);
    }
}
