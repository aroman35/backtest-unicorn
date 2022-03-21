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
        Logger = logger
            .ForContext<UpdateStateRequest<TState>>()
            .ForContext("GrainState", typeof(TState).Name);
    }

    private protected abstract UpdateDefinition<PersistenceModel<TState>> UpdateDefinition(TUpdateStateRequest request);

    private protected virtual Task HandlePreconditions(TUpdateStateRequest request, CancellationToken cancellationToke)
    {
        Logger.BindProperty("Id", request.Id, true, out _);
        Logger.Information("Preparing to update");
        return Task.CompletedTask;
    }
    
    private protected virtual Task<bool> HandleCallback(TUpdateStateRequest request, UpdateResult updateResult, CancellationToken cancellationToke)
    {
        if (updateResult.IsAcknowledged)
            Logger.Information("Successfully updated");
        else
            Logger.Warning("There was an error updating state");
        return Task.FromResult(updateResult.IsAcknowledged);
    }
    
    public virtual async Task<bool> Handle(TUpdateStateRequest request, CancellationToken cancellationToken)
    {
        await HandlePreconditions(request, cancellationToken);
        
        var updateResult = await Collection.UpdateOneAsync(request.Filter(), UpdateDefinition(request), new UpdateOptions
        {
            IsUpsert = IsUpsert
        }, cancellationToken);
        
        return await HandleCallback(request, updateResult, cancellationToken);
    }
}