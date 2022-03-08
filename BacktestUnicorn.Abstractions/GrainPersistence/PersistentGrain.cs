using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public class PersistentGrain<TState, TEvent> : JournaledGrain<TState, TEvent>, ICustomStorageInterface<TState, TEvent>
    where TState : State, new()
    where TEvent : EventBase<TState>
{
    private readonly IMediator _mediator;

    protected PersistentGrain(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<KeyValuePair<int, TState>> ReadStateFromStorage()
    {
        var state = await _mediator.Send(new StateRequest<TState>(this.GetPrimaryKey())) ?? new TState();
        return new KeyValuePair<int, TState>(1, state);
    }

    public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<TEvent> updates, int expectedversion)
    {
        var updateResult = true;
        foreach (var update in updates)
        {
            var command = update.Command(this.GetPrimaryKey());
            updateResult = updateResult && await _mediator.Send(command);
        }

        return updateResult;
    }
}