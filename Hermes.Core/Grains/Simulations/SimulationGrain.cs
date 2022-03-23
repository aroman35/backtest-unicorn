using Hermes.Abstractions.GrainInterfaces;
using Hermes.Abstractions.GrainObservers;
using Hermes.Abstractions.GrainPersistence;
using Hermes.Core.Common;
using Orleans.Providers;

namespace Hermes.Core.Grains.Simulations;

[LogConsistencyProvider(ProviderName = OrleansClusterConstants.StorageName)]
public class SimulationGrain : PersistentGrain<SimulationState, SimulationEventBase>, ISimulationGrain
{
    private IBacktestGrain _backtest;

    
    public Task Create(Guid backtestId)
    {
        _backtest = GrainFactory.GetGrain<IBacktestGrain>(backtestId);
        
        return Task.CompletedTask;
    }
}