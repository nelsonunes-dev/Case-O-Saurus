namespace CaseOSaurus.Application.CQRS.Interfaces;

public interface IQuery<TResponse> : ICommand<TResponse> { }
