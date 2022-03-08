using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public record StateRequest<TState>(Guid Id) : IRequest<TState>
    where TState : State;