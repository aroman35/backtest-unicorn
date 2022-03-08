using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Persistence.Internal.Models;

public abstract class PersistenceModel<TState>
    where TState : State
{
    public Guid Id { get; set; }
    public TState State { get; set; }
}