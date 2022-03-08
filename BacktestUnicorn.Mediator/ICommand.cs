namespace BacktestUnicorn.Mediator;

public interface ICommand
{
}

public interface ICommand<TResponse> : ICommand
{
}