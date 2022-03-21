using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Core.Grains.Jobs;

public abstract record JobEventBase(Guid GrainId) : EventBase<JobState>(GrainId);