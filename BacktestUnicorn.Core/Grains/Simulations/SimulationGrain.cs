using BacktestUnicorn.Abstractions.GrainInterfaces;
using BacktestUnicorn.Abstractions.GrainObservers;
using BacktestUnicorn.Abstractions.GrainPersistence;
using BacktestUnicorn.Core.Common;
using Orleans.Providers;

namespace BacktestUnicorn.Core.Grains.Simulations;

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