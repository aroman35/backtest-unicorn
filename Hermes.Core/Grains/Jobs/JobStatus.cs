namespace Hermes.Core.Grains.Jobs;

public enum JobStatus
{
    Queued = 0,
    Running = 1,
    Finished = 2,
    Canceled = 3,
    Failed = 4,
    NoMd = 5
}