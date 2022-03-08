using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Core.Grains.Jobs;

public class JobState : State
{
    public string Name { get; set; }
    public Guid AgentId { get; set; }
    public Guid SimulationId { get; set; }
    public JobStatus Status { get; set; }
    public DateTime? Started { get; set; }
    public DateTime? LastUpdate { get; set; }
    public double TestProgress { get; set; }
    public double CacheProgress { get; set; }
    public JobResult Result { get; set; }
}