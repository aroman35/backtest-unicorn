namespace BacktestUnicorn.Mediator;

public interface IMediatorServiceResolver
{
    ICommandHandler<TCommand> ResolveCommandHandler<TCommand>()
        where TCommand : ICommand;

    ICommandHandler<TCommand, TResponse> ResolveCommandHandler<TCommand, TResponse>()
        where TCommand : ICommand<TResponse>;

    IQueryHandler<TQuery, TResponse> ResolveQueryHandler<TQuery, TResponse>()
        where TQuery : IQuery<TResponse>;

    IStreamQueryHandler<TQuery, TResponse> ResolveQueryEnumerationHandler<TQuery, TResponse>()
        where TQuery : IStreamQuery<TResponse>;
}