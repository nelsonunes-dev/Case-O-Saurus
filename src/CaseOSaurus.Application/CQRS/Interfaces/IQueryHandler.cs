using CaseOSaurus.Application.CQRS.Interfaces;

namespace CaseOSaurus.Application.CQRS.Interfaces;

public interface IQueryHandler<TQuery, TResponse> : ICommandHandler<TQuery, TResponse> where TQuery : IQuery<TResponse> { }
