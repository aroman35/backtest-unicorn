namespace Hermes.Core.Grains.Jobs;

public class JobResultMessage
{
    public Guid JobId { get; set; }
    public Guid SimulationId { get; set; }
    public string Message { get; set; }
}