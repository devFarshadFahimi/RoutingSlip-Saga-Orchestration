using MassTransit;

namespace WebApi.SagaOrchestration.StateMachines;

public class SettlementState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    public string? FailedDescriptionCode { get; set; }
    public string? FailedDescriptionMessage { get; set; }
    public decimal Amount { get; set; }
}
