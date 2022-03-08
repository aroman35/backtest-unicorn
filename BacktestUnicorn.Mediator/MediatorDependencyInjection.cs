using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace BacktestUnicorn.Mediator;

public static class MediatorDependencyInjection
{
    public static IServiceCollection AddMediator(this IServiceCollection serviceCollection,
        params Assembly[] handlerAssemblies)
    {
        serviceCollection
            .AddScoped<IMediatorServiceResolver, MicrosoftDependencyInjectionServiceResolver>()
            .AddScoped<IMediator, Mediator>();

        var handlerTypes = handlerAssemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => (type.IsAssignableTo(typeof(ICommandHandler)) ||
                            type.IsAssignableTo(typeof(IQueryHandler)) ||
                            type.IsAssignableTo(typeof(IStreamQueryHandler))) &&
                           type.IsClass && !type.IsAbstract);

        foreach (var handlerType in handlerTypes)
        {
            var genericTypes = handlerType.GetInterfaces()
                                   .FirstOrDefault(x => x.IsGenericType)?
                                   .GenericTypeArguments
                               ?? throw new InvalidOperationException("Type is invalid");

            if (genericTypes.Length is not (2 or 1))
                throw new InvalidOperationException("Type is invalid");

            var handlerDescriptor = handlerType switch
            {
                var commandHandler
                    when commandHandler.IsAssignableTo(typeof(ICommandHandler)) &&
                         genericTypes[0].IsAssignableTo(typeof(ICommand)) =>
                    new ServiceDescriptor(
                        genericTypes.Length == 2
                            ? typeof(ICommandHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1])
                            : typeof(ICommandHandler<>).MakeGenericType(genericTypes[0]),
                        commandHandler,
                        ServiceLifetime.Scoped
                    ),
                var queryHandler
                    when queryHandler.IsAssignableTo(typeof(IQueryHandler)) &&
                         genericTypes[0].IsAssignableTo(typeof(IQuery)) =>
                    new ServiceDescriptor(
                        typeof(IQueryHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1]),
                        queryHandler,
                        ServiceLifetime.Scoped
                    ),
                var streamQueryHandler
                    when streamQueryHandler.IsAssignableTo(typeof(IStreamQueryHandler)) &&
                         genericTypes[0].IsAssignableTo(typeof(IStreamQuery)) =>
                    new ServiceDescriptor(
                        typeof(IStreamQueryHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1]),
                        streamQueryHandler,
                        ServiceLifetime.Scoped
                    ),
                _ => throw new InvalidOperationException($"Type {handlerType} is invalid")
            };
            serviceCollection.Add(handlerDescriptor); // IHandler Handler

            continue;
            ServiceDescriptor serviceDescriptor;

            if (handlerType.IsAssignableTo(typeof(ICommandHandler)))
            {
                if (!genericTypes[0].IsAssignableTo(typeof(ICommand)))
                    throw new InvalidOperationException("Type is invalid");

                var serviceType = genericTypes.Length == 2
                    ? typeof(ICommandHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1])
                    : typeof(ICommandHandler<>).MakeGenericType(genericTypes[0]);

                serviceDescriptor = new ServiceDescriptor(serviceType, handlerType, ServiceLifetime.Scoped);
            }

            else if (handlerType.IsAssignableTo(typeof(IQueryHandler)))
            {
                if (!genericTypes[0].IsAssignableTo(typeof(IQuery)))
                    throw new InvalidOperationException("Type is invalid");

                var serviceType = typeof(IQueryHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1]);
                serviceDescriptor = new ServiceDescriptor(serviceType, handlerType, ServiceLifetime.Scoped);
            }

            else if (handlerType.IsAssignableTo(typeof(IStreamQueryHandler)))
            {
                if (!genericTypes[0].IsAssignableTo(typeof(IStreamQuery)))
                    throw new InvalidOperationException("Type is invalid");

                var serviceType = typeof(IStreamQueryHandler<,>).MakeGenericType(genericTypes[0], genericTypes[1]);
                serviceDescriptor = new ServiceDescriptor(serviceType, handlerType, ServiceLifetime.Scoped);
            }

            else
            {
                throw new InvalidOperationException("Type is invalid");
            }

            serviceCollection.Add(serviceDescriptor);
        }

        return serviceCollection;
    }
}