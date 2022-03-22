using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public abstract record UpdateStateCommand<TState>(Guid Id, int Version) : IRequest<bool>
    where TState : State;

public class JournalEvent
{
    public DateTime EventTime { get; set; }
    public string Message { get; set; }
    public bool IsSuccess { get; set; }
}

public abstract record UpdateJournalStateCommand<TState>(Guid Id, int Version) : UpdateStateCommand<TState>(Id, Version)
    where TState : State
{
    public abstract JournalEvent Event { get; }
}