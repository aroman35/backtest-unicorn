using Orleans;

namespace BacktestUnicorn.Abstractions.GrainInterfaces;

public interface IJobGrain : IGrainWithGuidKey
{
    Task Create(Guid simulationId);
    Task UpdateTestProgress(double progress);
}