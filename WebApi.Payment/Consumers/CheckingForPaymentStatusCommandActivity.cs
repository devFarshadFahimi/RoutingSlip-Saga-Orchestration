using MassTransit;
using SharedContracts.Commands;

namespace WebApi.Payment.Consumers;

// step 2 => going to Payment to check payment 
public class CheckingForPaymentStatusCommandActivity : IActivity<CheckingForPaymentStatusCommand, CheckingForPaymentStatusCommand>
{
    public async Task<CompensationResult> Compensate(CompensateContext<CheckingForPaymentStatusCommand> context)
    {
        await Task.Delay(100);
        return context.Compensated();
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<CheckingForPaymentStatusCommand> context)
    {
        await Task.Delay(111);
        return context.Completed(new CheckingForPaymentStatusCommand
        {
            Amount = context.Arguments.Amount,
            CustomerId = context.Arguments.CustomerId,
            SrcIban = "Some IBAN",
            TrackingCode = context.CorrelationId!.Value,
        });
    }
}
