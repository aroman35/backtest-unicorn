using Hermes.Abstractions.GrainPersistence;

namespace Hermes.Abstractions.StateManagement;

public abstract record EventBase<TState>
    where TState : State
{
    public abstract UpdateStateCommand<TState> Command(int version, Guid id);
}