using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Persistence.Internal.Models;
using MediatR;
using MongoDB.Driver;
using Serilog;

namespace BacktestUnicorn.Persistence.Internal;

public class StateQueryHandlerBase<TStateQuery, TState> : IRequestHandler<TStateQuery, KeyValuePair<int, TState>>
    where TState : State, new()
    where TStateQuery : StateQuery<TState>
{
    private protected readonly IMongoDbContext MongoDbContext;
    private protected readonly ILogger Logger;

    public StateQueryHandlerBase(IMongoDbContext mongoDbContext, ILogger logger)
    {
        MongoDbContext = mongoDbContext;
        Logger = logger
            .ForContext<StateQueryHandlerBase<TStateQuery, TState>>()
            .ForContext("GrainState", typeof(TState).Name);
    }

    private protected IMongoCollection<PersistenceModel<TState>> Collection => MongoDbContext.Collection<TState>();

    public virtual async Task<KeyValuePair<int, TState>> Handle(TStateQuery request, CancellationToken cancellationToken)
    {
        var filter = Builders<PersistenceModel<TState>>.Filter.Eq(x => x.Id, request.Id);
        using var findCursor = await Collection.FindAsync(filter, new FindOptions<PersistenceModel<TState>>
        {
            AllowDiskUse = false,
            AllowPartialResults = false,
            Limit = 1
        }, cancellationToken);
        var foundDocument = await findCursor.FirstOrDefaultAsync(cancellationToken);
        
        return foundDocument is null
            ? new KeyValuePair<int, TState>(1, new TState())
            : new KeyValuePair<int, TState>(foundDocument.Version, foundDocument.State);
    }
}