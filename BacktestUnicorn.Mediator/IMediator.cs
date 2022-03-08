namespace BacktestUnicorn.Mediator;

public interface IMediator
{
    Task<TResponse> Query<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IQuery<TResponse>;

    Task SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand;

    Task<TResponse> SendCommand<TCommand, TResponse>(TCommand command, CancellationToken cancellationToken)
        where TCommand : ICommand<TResponse>;

    IAsyncEnumerable<TResponse> QueryStream<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken)
        where TQuery : IStreamQuery<TResponse>;
}