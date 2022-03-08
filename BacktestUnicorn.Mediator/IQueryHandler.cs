namespace BacktestUnicorn.Mediator;

public interface IQueryHandler
{
}

public interface IQueryHandler<in TQuery, TResponse> : IQueryHandler
    where TQuery : IQuery<TResponse>
{
    Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken);
}