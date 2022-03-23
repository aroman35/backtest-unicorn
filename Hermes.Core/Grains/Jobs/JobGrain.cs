using Hermes.Abstractions.GrainInterfaces;
using Hermes.Abstractions.GrainPersistence;
using Hermes.Core.Common;
using Hermes.Core.Grains.Jobs.Commands;
using Orleans.Providers;

namespace Hermes.Core.Grains.Jobs;

[LogConsistencyProvider(ProviderName = OrleansClusterConstants.StorageName)]
public class JobGrain : PersistentGrain<JobState, JobEventBase>, IJobGrain
{
    private ISimulationGrain _simulation;
    public async Task Create(Guid simulationId)
    {
        _simulation = GrainFactory.GetGrain<ISimulationGrain>(simulationId);
        RaiseEvent(new AttachSimulationEvent(simulationId));
        await ConfirmEvents();
    }

    public Task UpdateTestProgress(double progress)
    {
        RaiseEvent(new UpdateTestProgressEvent(progress));
        return Task.CompletedTask;
    }
}