using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public abstract record UpdateStateRequest<TState>(Guid Id) : IRequest<bool>
    where TState : State;