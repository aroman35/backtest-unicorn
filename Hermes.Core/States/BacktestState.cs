using Hermes.Abstractions.StateManagement;

namespace Hermes.Core.States;

public class BacktestState : State
{
    public BacktestStartOptions StartOptions { get; set; }
    public string Name { get; set; }
    public int TotalJobs { get; set; }
    public int FinishedJobs { get; set; }
    public int TotalSimulations { get; set; }
    public string BinaryName { get; set; }
    public double Progress { get; set; }
    public DateTime Created { get; set; }
    public DateTime MarketDataStartDate { get; set; }
    public DateTime MarketDataEndDate { get; set; }
    public string OptimizationParams { get; set; }
    public string BacktestSettings { get; set; }
    public string[] StrategySettings { get; set; }
    public Guid[] AgentIds { get; set; }
}

public class BacktestStartOptions
{
    public int RetryCount { get; set; }
    public double ReportingTimeout { get; set; }
    public bool EnableLogs { get; set; }
    public bool EnableTelemetry { get; set; }
    public bool SplitDays { get; set; }
}