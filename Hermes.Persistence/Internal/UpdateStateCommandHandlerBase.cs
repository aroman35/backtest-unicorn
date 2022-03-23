using Hermes.Abstractions.GrainPersistence;
using Hermes.Abstractions.StateManagement;
using Hermes.Persistence.Internal.Models;
using MediatR;
using MongoDB.Driver;
using Serilog;

namespace Hermes.Persistence.Internal;

public abstract class UpdateStateCommandHandlerBase<TUpdateStateRequest, TState> : IRequestHandler<TUpdateStateRequest, bool>
    where TState : State
    where TUpdateStateRequest : UpdateStateCommand<TState>
{
    private protected readonly IMongoDbContext MongoDbContext;
    private protected readonly ILogger Logger;
    private protected readonly UpdateDefinitionBuilder<PersistenceModel<TState>> UpdateDefinitionBuilder = Builders<PersistenceModel<TState>>.Update;
    private protected IMongoCollection<PersistenceModel<TState>> Collection => MongoDbContext.Collection<TState>();
    private protected virtual bool IsUpsert => true;

    protected UpdateStateCommandHandlerBase(IMongoDbContext mongoDbContext, ILogger logger)
    {
        MongoDbContext = mongoDbContext;
        Logger = logger
            .ForContext<UpdateStateCommandHandlerBase<TUpdateStateRequest, TState>>()
            .ForContext("GrainState", typeof(TState).Name);
    }

    private protected abstract UpdateDefinition<PersistenceModel<TState>> UpdateDefinition(TUpdateStateRequest request);

    private protected virtual Task HandlePreconditions(TUpdateStateRequest request, CancellationToken cancellationToken)
    {
        Logger.BindProperty("Id", request.Id, true, out _);
        Logger.Information("Preparing to update");

        return Task.CompletedTask;
    }

    private async Task AttachJournalEvent(JournalModel @event, CancellationToken cancellationToken)
    {
        var journal = MongoDbContext.Journal<TState>();
        await journal.InsertOneAsync(@event, new InsertOneOptions
        {
            BypassDocumentValidation = true
        }, cancellationToken);
    }
    
    private protected virtual async Task<bool> HandleCallback(TUpdateStateRequest request, UpdateResult updateResult, CancellationToken cancellationToken)
    {
        if (updateResult.IsAcknowledged)
            Logger.Information("Successfully updated");
        else
            Logger.Warning("There was an error updating state");
        
        if (request is UpdateJournalStateCommand<TState> journalRequest)
            await AttachJournalEvent(new JournalModel(journalRequest.Event), cancellationToken);
        
        return updateResult.IsAcknowledged;
    }

    public virtual async Task<bool> Handle(TUpdateStateRequest request, CancellationToken cancellationToken)
    {
        await HandlePreconditions(request, cancellationToken);

        var updateDefinition = Builders<PersistenceModel<TState>>.Update.Combine(
            Builders<PersistenceModel<TState>>.Update.Inc(x => x.Version, 1),
            UpdateDefinition(request)
        );

        var findResult = await Collection.FindOneAndUpdateAsync(
            request.Filter(),
            updateDefinition,
            new FindOneAndUpdateOptions<PersistenceModel<TState>>
            {
                IsUpsert = IsUpsert
            }, cancellationToken);

        var version = findResult.Version;
        var updateResult = await Collection.UpdateOneAsync(
            request.Filter(),
            updateDefinition,
            new UpdateOptions
            {
                IsUpsert = IsUpsert
            }, cancellationToken);

        return await HandleCallback(request, updateResult, cancellationToken);
    }
}