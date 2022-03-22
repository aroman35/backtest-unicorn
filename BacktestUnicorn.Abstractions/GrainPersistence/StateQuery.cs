using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public record StateQuery<TState>(Guid Id) : IRequest<KeyValuePair<int, TState>>
    where TState : State;