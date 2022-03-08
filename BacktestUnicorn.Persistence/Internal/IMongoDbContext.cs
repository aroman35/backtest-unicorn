using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Persistence.Internal.Models;
using MongoDB.Driver;

namespace BacktestUnicorn.Persistence.Internal;

public interface IMongoDbContext
{
    IMongoCollection<PersistenceModel<TState>> Collection<TState>()
        where TState : State;
}