using BacktestUnicorn.Abstractions.GrainPersistence;

namespace BacktestUnicorn.Core.Grains.Jobs.Commands;

public record AttachSimulationRequest(Guid Id, Guid SimulationId) : UpdateStateRequest<JobState>(Id);

public record AttachSimulationEvent(Guid GrainId, Guid SimulationId) : JobEventBase(GrainId)
{
    public override UpdateStateRequest<JobState> Command() => new AttachSimulationRequest(GrainId, SimulationId);
}