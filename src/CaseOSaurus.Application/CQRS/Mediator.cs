using CaseOSaurus.Application.CQRS.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CaseOSaurus.Application.CQRS;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
    {
        var requestType = command.GetType();
        var handlerType = typeof(ICommandHandler<,>).MakeGenericType(requestType, typeof(TResponse));
        dynamic handler = _serviceProvider.GetRequiredService(handlerType);

        RequestHandlerDelegate<TResponse> pipeline = () => handler.Handle((dynamic)command, cancellationToken);

        // Get all pipeline behaviors for this request/response pair
        var behaviourType = typeof(IPipelineBehavior<,>).MakeGenericType(requestType, typeof(TResponse));
        var behaviours = _serviceProvider.GetServices(behaviourType)
            .Cast<object>()
            .Reverse();

        foreach (var behaviourTypes in behaviours)
        {
            var behaviour = (dynamic)behaviourTypes;
            var next = pipeline;
            pipeline = () => behaviour.Handle((dynamic)command, next, cancellationToken);
        }

        return await pipeline();
    }
}
