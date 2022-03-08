using Orleans;

namespace BacktestUnicorn.Abstractions.Observers;

public interface IStrategyObserver : IGrainObserver
{
    void Stop();
}