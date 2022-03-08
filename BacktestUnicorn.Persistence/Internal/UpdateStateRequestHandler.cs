using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Persistence.Internal.Models;
using MediatR;
using MongoDB.Driver;
using Serilog;

namespace BacktestUnicorn.Persistence.Internal;

public abstract class UpdateStateRequestHandler<TUpdateStateRequest, TState> : IRequestHandler<TUpdateStateRequest, bool>
    where TState : State
    where TUpdateStateRequest : UpdateStateRequest<TState>
{
    private protected readonly IMongoDbContext MongoDbContext;
    private protected readonly ILogger Logger;
    private protected readonly UpdateDefinitionBuilder<PersistenceModel<TState>> UpdateDefinitionBuilder = Builders<PersistenceModel<TState>>.Update;
    private protected IMongoCollection<PersistenceModel<TState>> Collection => MongoDbContext.Collection<TState>();
    private protected virtual bool IsUpsert => true;

    protected UpdateStateRequestHandler(IMongoDbContext mongoDbContext, ILogger logger)
    {
        MongoDbContext = mongoDbContext;
        Logger = logger.ForContext<UpdateStateRequest<TState>>();
    }

    private protected abstract UpdateDefinition<PersistenceModel<TState>> UpdateDefinition(TUpdateStateRequest request);

    private protected virtual Task HandlePreconditions(TUpdateStateRequest request, CancellationToken cancellationToke)
    {
        return Task.CompletedTask;
    }
    
    private protected virtual Task HandleCallback(TUpdateStateRequest request, UpdateResult updateResult, CancellationToken cancellationToke)
    {
        return Task.CompletedTask;
    }
    
    public virtual async Task<bool> Handle(TUpdateStateRequest request, CancellationToken cancellationToken)
    {
        await HandlePreconditions(request, cancellationToken);
        Logger.Information("Preparing to update {StateName}", typeof(TState).Name);
        var updateResult = await Collection.UpdateOneAsync(request.Filter(), UpdateDefinition(request), new UpdateOptions
        {
            IsUpsert = IsUpsert
        }, cancellationToken);

        if (updateResult.IsAcknowledged)
            Logger.Information("{StateName} was successfully updated", typeof(TState).Name);
        else
            Logger.Warning("There was an error updating state for {StateName}", typeof(TState).Name);
        await HandleCallback(request, updateResult, cancellationToken);
        return updateResult.IsAcknowledged;
    }
}