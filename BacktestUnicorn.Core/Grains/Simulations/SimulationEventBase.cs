using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Core.Grains.Jobs;

namespace BacktestUnicorn.Core.Grains.Simulations;

public abstract record SimulationEventBase(Guid GrainId) : EventBase<SimulationState>(GrainId);