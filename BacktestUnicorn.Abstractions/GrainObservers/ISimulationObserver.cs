using Orleans;

namespace BacktestUnicorn.Abstractions.GrainObservers;

public interface ISimulationObserver : IGrainObserver
{
    void Stop();
}