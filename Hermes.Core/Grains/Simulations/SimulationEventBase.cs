using Hermes.Abstractions.StateManagement;
using Hermes.Core.Grains.Jobs;

namespace Hermes.Core.Grains.Simulations;

public abstract record SimulationEventBase : EventBase<SimulationState>;