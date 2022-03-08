using Orleans;

namespace BacktestUnicorn.Abstractions.GrainInterfaces;

public interface IJobGrain : IGrainWithGuidKey
{
    Task Attach(Guid simulationId);
}