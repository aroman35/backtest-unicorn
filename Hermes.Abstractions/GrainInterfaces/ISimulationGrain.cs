using Orleans;

namespace Hermes.Abstractions.GrainInterfaces;

public interface ISimulationGrain : IGrainWithGuidKey
{
    Task Create(Guid backtestId);
}