using BacktestUnicorn.Abstractions.GrainPersistence;

namespace BacktestUnicorn.Core.Grains.Jobs.Commands;

public record AttachSimulationRequest(Guid Id, Guid SimulationId) : UpdateStateRequest<JobState>(Id);

public record AttachSimulationEvent(Guid SimulationId) : JobEventBase
{
    public override UpdateStateRequest<JobState> Command(Guid grainId) => new AttachSimulationRequest(grainId, SimulationId);
}