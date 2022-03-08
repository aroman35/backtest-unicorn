using BacktestUnicorn.Abstractions.GrainInterfaces;
using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Core.Grains.Jobs.Commands;
using MediatR;
using Orleans.Providers;

namespace BacktestUnicorn.Core.Grains.Jobs;

[LogConsistencyProvider(ProviderName = "CustomStorage")]
public class JobGrain : PersistentGrain<JobState, JobEventBase>, IJobGrain
{
    private ISimulationGrain _simulation;
    private IBacktestGrain _backtest;
    
    public JobGrain(IMediator mediator) : base(mediator)
    {
    }

    public async Task Attach(Guid simulationId)
    {
        _simulation = GrainFactory.GetGrain<ISimulationGrain>(simulationId);
        _backtest = GrainFactory.GetGrain<IBacktestGrain>(await _simulation.GroupId());
        RaiseEvent(new AttachSimulationEvent(simulationId));
    }
}