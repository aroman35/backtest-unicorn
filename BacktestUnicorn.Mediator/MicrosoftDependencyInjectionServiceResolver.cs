using Microsoft.Extensions.DependencyInjection;

namespace BacktestUnicorn.Mediator;

public class MicrosoftDependencyInjectionServiceResolver : IMediatorServiceResolver
{
    private readonly IServiceProvider _serviceProvider;

    public MicrosoftDependencyInjectionServiceResolver(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ICommandHandler<TCommand> ResolveCommandHandler<TCommand>() where TCommand : ICommand
    {
        return _serviceProvider.GetRequiredService<ICommandHandler<TCommand>>();
    }

    public ICommandHandler<TCommand, TResponse> ResolveCommandHandler<TCommand, TResponse>()
        where TCommand : ICommand<TResponse>
    {
        return _serviceProvider.GetRequiredService<ICommandHandler<TCommand, TResponse>>();
    }

    public IQueryHandler<TQuery, TResponse> ResolveQueryHandler<TQuery, TResponse>()
        where TQuery : IQuery<TResponse>
    {
        return _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TResponse>>();
    }

    public IStreamQueryHandler<TQuery, TResponse> ResolveQueryEnumerationHandler<TQuery, TResponse>()
        where TQuery : IStreamQuery<TResponse>
    {
        return _serviceProvider.GetRequiredService<IStreamQueryHandler<TQuery, TResponse>>();
    }
}