using System.Runtime.CompilerServices;

namespace BacktestUnicorn.Mediator;

public class Mediator : IMediator
{
    private readonly IMediatorServiceResolver _serviceResolver;

    public Mediator(IMediatorServiceResolver serviceResolver)
    {
        _serviceResolver = serviceResolver;
    }

    public async Task<TResponse> Query<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>
    {
        var handler = _serviceResolver.ResolveQueryHandler<TQuery, TResponse>();
        var response = await handler.Handle(query, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand
    {
        var handler = _serviceResolver.ResolveCommandHandler<TCommand>();
        await handler.Handle(command, cancellationToken);
    }

    public async Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>
    {
        var handler = _serviceResolver.ResolveCommandHandler<TCommand, TResponse>();
        var response = await handler.Handle(command, cancellationToken);
        return response;
    }

    public async IAsyncEnumerable<TResponse> QueryStream<TQuery, TResponse>(TQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken)
        where TQuery : IStreamQuery<TResponse>
    {
        var handler = _serviceResolver.ResolveQueryEnumerationHandler<TQuery, TResponse>();
        await using var enumerator = handler.Handle(query, cancellationToken).GetAsyncEnumerator(cancellationToken);
        while (await enumerator.MoveNextAsync())
            yield return enumerator.Current;
    }
}