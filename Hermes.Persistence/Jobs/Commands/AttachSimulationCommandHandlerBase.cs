using Hermes.Core.Grains.Jobs;
using Hermes.Core.Grains.Jobs.Commands;
using Hermes.Persistence.Internal;
using Hermes.Persistence.Internal.Models;
using MongoDB.Driver;
using Serilog;

namespace Hermes.Persistence.Jobs.Commands;

public class AttachSimulationCommandHandlerBase : UpdateStateCommandHandlerBase<AttachSimulationCommand, JobState>
{
    public AttachSimulationCommandHandlerBase(IMongoDbContext mongoDbContext, ILogger logger) : base(mongoDbContext, logger)
    {
    }
    
    private protected override UpdateDefinition<PersistenceModel<JobState>> UpdateDefinition(AttachSimulationCommand command)
    {
        var updateDefinition = UpdateDefinitionBuilder.Set(x => x.State.SimulationId, command.SimulationId);
        return updateDefinition;
    }
}

public class UpdateTestProgressCommandHandler : UpdateStateCommandHandlerBase<UpdateJournalTestProgressCommand, JobState>
{
    public UpdateTestProgressCommandHandler(IMongoDbContext mongoDbContext, ILogger logger) : base(mongoDbContext, logger)
    {
    }

    private protected override UpdateDefinition<PersistenceModel<JobState>> UpdateDefinition(UpdateJournalTestProgressCommand command)
    {
        var updateDefinition = UpdateDefinitionBuilder.Inc(x => x.State.TestProgress, command.Progress);
        return updateDefinition;
    }
}