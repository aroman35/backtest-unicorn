namespace BacktestUnicorn.Core.Grains.Jobs;

public class JobResult
{
    public bool IsMdExists { get; set; }
    public bool IsSuccess { get; set; }
    public long MdEntryCount { get; set; }
    public double TestDuration { get; set; }
    public double? TaskDuration { get; set; }
}