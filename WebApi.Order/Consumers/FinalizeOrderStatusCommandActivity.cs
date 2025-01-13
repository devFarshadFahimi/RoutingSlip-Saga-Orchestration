using MassTransit;
using SharedContracts.Commands;

namespace WebApi.Order.Consumers;

// step 3 => going back to Order to finalize status
public class FinalizeOrderStatusCommandActivity : IActivity<FinalizeOrderStatusCommand, FinalizeOrderStatusCommand>
{
    public Task<CompensationResult> Compensate(CompensateContext<FinalizeOrderStatusCommand> context)
    {
        throw new NotImplementedException();
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<FinalizeOrderStatusCommand> context)
    {
        await Task.Delay(100);
        if (Random.Shared.Next(1, 10) < 5)
        {
            return context.Faulted();
        }
        return context.Completed();
    }
}
