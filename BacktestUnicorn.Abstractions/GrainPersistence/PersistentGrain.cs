using BacktestUnicorn.Abstractions.StateManagement;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace BacktestUnicorn.Abstractions.GrainPersistence;

public class PersistentGrain<TState, TEvent> : JournaledGrain<TState, TEvent>, ICustomStorageInterface<TState, TEvent>
    where TState : State, new()
    where TEvent : EventBase<TState>
{
    public async Task<KeyValuePair<int, TState>> ReadStateFromStorage()
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var state = await mediator.Send(new StateRequest<TState>(this.GetPrimaryKey())) ?? new TState();
        return new KeyValuePair<int, TState>(1, state);
    }

    public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<TEvent> updates, int expectedversion)
    {
        var updateResult = true;
        
        await using var scope = ServiceProvider.CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        foreach (var update in updates)
        {
            var command = update.Command();
            updateResult = updateResult && await mediator.Send(command);
        }

        return updateResult;
    }
}