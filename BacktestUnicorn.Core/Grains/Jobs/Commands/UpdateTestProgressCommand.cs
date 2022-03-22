using BacktestUnicorn.Abstractions.GrainPersistence;

namespace BacktestUnicorn.Core.Grains.Jobs.Commands;

public record UpdateJournalTestProgressCommand(Guid Id, int Version, double Progress) : UpdateJournalStateCommand<JobState>(Id, Version)
{
    public override JournalEvent Event => new()
    {
        EventTime = DateTime.UtcNow,
        IsSuccess = true,
        Message = $"Test progress updated: {Math.Round(Progress, 2, MidpointRounding.ToZero)}"
    };
}

public record UpdateTestProgressEvent(double Progress) : JobEventBase
{
    public override UpdateStateCommand<JobState> Command(int version, Guid id) => new UpdateJournalTestProgressCommand(id, version, Progress);
}