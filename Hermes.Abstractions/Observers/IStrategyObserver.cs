using Orleans;

namespace Hermes.Abstractions.Observers;

public interface IStrategyObserver : IGrainObserver
{
    void Stop();
}