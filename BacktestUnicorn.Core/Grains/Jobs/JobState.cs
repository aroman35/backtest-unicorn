using BacktestUnicorn.Abstractions.StateManagement;
using BacktestUnicorn.Core.Grains.Jobs.Commands;

namespace BacktestUnicorn.Core.Grains.Jobs;

public class JobState : State
{
    public string Name { get; private set; }
    public Guid AgentId { get; private set; }
    public Guid SimulationId { get; private set; }
    public JobStatus Status { get; private set; }
    public DateTime? Started { get; private set; }
    public DateTime? LastUpdate { get; private set; }
    public double TestProgress { get; private set; }
    public double CacheProgress { get; private set; }
    public JobResult Result { get; private set; }

    public void Apply(AttachSimulationEvent @event)
    {
        SimulationId = @event.SimulationId;
    }
}