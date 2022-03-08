using BacktestUnicorn.Abstractions.GrainPersistence;

namespace BacktestUnicorn.Abstractions.StateManagement;

public abstract record EventBase<TState> where TState : State
{
    public abstract UpdateStateRequest<TState> Command(Guid grainId);
}