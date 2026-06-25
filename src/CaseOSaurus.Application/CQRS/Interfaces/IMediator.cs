namespace CaseOSaurus.Application.CQRS.Interfaces;

public interface IMediator
{
    Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);
}
