using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Core.Grains.Jobs;

namespace BacktestUnicorn.Core.Grains.Simulations;

public abstract record SimulationEventBase : EventBase<SimulationState>;