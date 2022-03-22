using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Persistence.Internal.Models;

public class PersistenceModel<TState>
    where TState : State
{
    public Guid Id { get; private set; }
    public int Version { get; private set; }
    public TState State { get; private set; }
}

public class JournalModel
{
    public JournalModel()
    {
    }
    public JournalModel(JournalEvent @event)
    {
        Event = @event;
    }
    public JournalEvent Event { get; private set; }
}