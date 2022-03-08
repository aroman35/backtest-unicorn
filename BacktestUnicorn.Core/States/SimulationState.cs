using BacktestUnicorn.Abstractions.StateManagement;

namespace BacktestUnicorn.Core.States;

public class SimulationState : State
{
    public string Name { get; set; }
    public Guid GroupId { get; set; }
    public int TotalJobs { get; set; }
    public int FinishedJobs { get; set; }
    public double Progress { get; set; }
    public double Speed { get; set; }
    public DateTime MarketDataStartDate { get; set; }
    public DateTime MarketDataEndDate { get; set; }
    public Dictionary<string, object> OptimizationParams { get; set; }
}