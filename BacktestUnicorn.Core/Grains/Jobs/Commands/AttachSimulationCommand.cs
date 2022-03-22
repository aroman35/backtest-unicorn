using BacktestUnicorn.Abstractions.GrainPersistence;

namespace BacktestUnicorn.Core.Grains.Jobs.Commands;

public record AttachSimulationCommand(Guid Id, int Version, Guid SimulationId) : UpdateStateCommand<JobState>(Id, Version);

public record AttachSimulationEvent(Guid SimulationId) : JobEventBase
{
    public override UpdateStateCommand<JobState> Command(int version, Guid id) => new AttachSimulationCommand(id, version, SimulationId);
}