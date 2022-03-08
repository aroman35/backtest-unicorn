namespace BacktestUnicorn.Mediator;

public interface IStreamQueryHandler
{
}

public interface IStreamQueryHandler<in TQuery, out TResponse> : IStreamQueryHandler
    where TQuery : IStreamQuery<TResponse>
{
    IAsyncEnumerable<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}