using MassTransit;
using MassTransit.EntityFrameworkCoreIntegration;
using Microsoft.EntityFrameworkCore;
using WebApi.SagaOrchestration.StateMachines;

namespace WebApi.SagaOrchestration.DataLayer;

public class SagaContext(DbContextOptions<SagaContext> options) : SagaDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }
    protected override IEnumerable<ISagaClassMap> Configurations
    {
        get
        {
            yield return new SettlementStateMachineMap();
        }
    }
}