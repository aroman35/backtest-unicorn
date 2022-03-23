using Hermes.Abstractions.StateManagement;
using MediatR;

namespace Hermes.Abstractions.GrainPersistence;

public record StateQuery<TState>(Guid Id) : IRequest<KeyValuePair<int, TState>>
    where TState : State;