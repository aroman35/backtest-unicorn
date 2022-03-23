using Hermes.Abstractions.StateManagement;
using Hermes.Persistence.Internal.Models;
using MongoDB.Driver;

namespace Hermes.Persistence.Internal;

public interface IMongoDbContext
{
    IMongoCollection<PersistenceModel<TState>> Collection<TState>()
        where TState : State;
    
    IMongoCollection<JournalModel> Journal<TState>()
        where TState : State;
}