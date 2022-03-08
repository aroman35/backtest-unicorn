using Orleans;

namespace BacktestUnicorn.Abstractions.GrainInterfaces;

public interface ISimulationGrain : IGrainWithGuidKey
{
    Task<Guid> GroupId();
}