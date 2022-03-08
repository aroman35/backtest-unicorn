using BacktestUnicorn.Core.Grains.Jobs;
using BacktestUnicorn.Core.Grains.Jobs.Commands;
using BacktestUnicorn.Persistence.Internal;
using BacktestUnicorn.Persistence.Internal.Models;
using MongoDB.Driver;
using Serilog;

namespace BacktestUnicorn.Persistence.Jobs.Commands;

public class AttachSimulationRequestHandler : UpdateStateRequestHandler<AttachSimulationRequest, JobState>
{
    public AttachSimulationRequestHandler(IMongoDbContext mongoDbContext, ILogger logger) : base(mongoDbContext, logger)
    {
    }
    
    private protected override UpdateDefinition<PersistenceModel<JobState>> UpdateDefinition(AttachSimulationRequest request)
    {
        var updateDefinition = UpdateDefinitionBuilder.Set(x => x.State.SimulationId, request.SimulationId);
        return updateDefinition;
    }
}