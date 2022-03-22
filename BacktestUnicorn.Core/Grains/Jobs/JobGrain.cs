using BacktestUnicorn.Abstractions.GrainInterfaces;
using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Core.Common;
using BacktestUnicorn.Core.Grains.Jobs.Commands;
using Orleans.Providers;

namespace BacktestUnicorn.Core.Grains.Jobs;

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