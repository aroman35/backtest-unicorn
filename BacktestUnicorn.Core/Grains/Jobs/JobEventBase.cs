using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Core.Grains.Jobs;

public abstract record JobEventBase : EventBase<JobState>;