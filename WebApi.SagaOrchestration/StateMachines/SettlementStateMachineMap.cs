using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebApi.SagaOrchestration.StateMachines;

public class SettlementStateMachineMap : SagaClassMap<SettlementState>
{
    protected override void Configure(EntityTypeBuilder<SettlementState> entity, ModelBuilder model)
    {
        base.Configure(entity, model);
    }
}