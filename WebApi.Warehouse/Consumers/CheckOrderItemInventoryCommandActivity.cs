using MassTransit;
using SharedContracts.Commands;

namespace WebApi.Warehouse.Consumers;

// step 1 => going to Warehouse to check inventory
public class CheckOrderItemInventoryCommandActivity : IActivity<CheckOrderItemInventoryCommand, CheckOrderItemInventoryCommand>
{
    public async Task<CompensationResult> Compensate(CompensateContext<CheckOrderItemInventoryCommand> context)
    {
        await Task.Delay(100);
        return context.Compensated();
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<CheckOrderItemInventoryCommand> context)
    {
        await Task.Delay(111);
        return context.Completed(new CheckOrderItemInventoryCommand
        {
            Amount = context.Arguments.Amount,
            TrackingCode = context.CorrelationId!.Value,
        });
    }
}
