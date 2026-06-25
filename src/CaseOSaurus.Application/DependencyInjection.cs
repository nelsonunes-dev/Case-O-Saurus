using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using CaseOSaurus.Application.CQRS;
using CaseOSaurus.Application.CQRS.Interfaces;
using CaseOSaurus.Application.CQRS.PipelineBehaviors;

namespace CaseOSaurus.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg => cfg.AddMaps(typeof(DependencyInjection).Assembly));

        // FluentValidation
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        // Mediator
        services.AddScoped<IMediator, Mediator>();

        // Pipeline Behaviors
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));

        // Register all ICommandHandler implementations in this assembly
        var assembly = typeof(DependencyInjection).Assembly;
        var handlerTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .Select(t => new { Implementation = t, Interfaces = t.GetInterfaces() })
            .Where(x => x.Interfaces.Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

        foreach (var ht in handlerTypes)
        {
            foreach (var iface in ht.Interfaces.Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            {
                services.AddScoped(iface, ht.Implementation);
            }
        }

        return services;
    }
}
