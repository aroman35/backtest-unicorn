using Hermes.Abstractions.StateManagement;

namespace Hermes.Core.Grains.Jobs;

public abstract record JobEventBase : EventBase<JobState>;