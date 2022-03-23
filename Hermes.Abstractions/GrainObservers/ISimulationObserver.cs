using Orleans;

namespace Hermes.Abstractions.GrainObservers;

public interface ISimulationObserver : IGrainObserver
{
    void Stop();
}