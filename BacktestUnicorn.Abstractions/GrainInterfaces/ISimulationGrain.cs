using Orleans;

namespace BacktestUnicorn.Abstractions.GrainInterfaces;

public interface ISimulationGrain : IGrainWithGuidKey
{
    Task Create(Guid backtestId);
}