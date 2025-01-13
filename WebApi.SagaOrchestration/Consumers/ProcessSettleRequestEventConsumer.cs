using MassTransit;
using MassTransit.Courier.Contracts;
using SharedContracts;
using SharedContracts.Commands;
using SharedContracts.Events;
using WebApi.SagaOrchestration.DataLayer;

namespace WebApi.SagaOrchestration.Consumers;

public class ProcessSettleRequestEventConsumer(IEndpointAddressProvider provider) : IConsumer<ProcessSettleRequestEvent>
{
    public async Task Consume(ConsumeContext<ProcessSettleRequestEvent> context)
    {
        var routingSlip = await CreateRoutingSlip(context);
        await context.Execute(routingSlip).ConfigureAwait(false);
    }

    async Task<RoutingSlip> CreateRoutingSlip(ConsumeContext<ProcessSettleRequestEvent> context)
    {
        var builder = new RoutingSlipBuilder(NewId.NextGuid());

        builder.SetVariables(new
        {
            context.SourceAddress,
            context.Message.TrackingCode,
            context.Message.Amount,
        });

        #region Old
        //Success Route

        // step 1 => going to Warehouse to check inventory
        var checkAccountBalanceAndSubmitTransactionCommandActivityUrl = QueueNames.GetActivityUri(nameof(CheckOrderItemInventoryCommand));
        builder.AddActivity("CheckOrderItemInventoryCommandActivity", checkAccountBalanceAndSubmitTransactionCommandActivityUrl);

        // step 2 => going to Payment to check payment 
        var checkingForPaymentStatusCommandActivityUri = QueueNames.GetActivityUri(nameof(CheckingForPaymentStatusCommand));
        builder.AddActivity("CheckingForPaymentStatusCommandActivity", checkingForPaymentStatusCommandActivityUri);

        // step 3 => going back to Order to finalize status
        var finalizeOrderStatusCommandActivityUri = QueueNames.GetActivityUri(nameof(FinalizeOrderStatusCommand));
        builder.AddActivity("FinalizeOrderStatusCommandActivity", finalizeOrderStatusCommandActivityUri);

        await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Completed,
             x => x.Send(new SettlementRequestSucceededEvent
             {
                 TrackingCode = context.Message.TrackingCode
             }));

        await builder.AddSubscription(context.SourceAddress, RoutingSlipEvents.Faulted,
             x => x.Send(new SettlementRequestFailedEvent
             {
                 TrackingCode = context.Message.TrackingCode,
                 FailedDescriptionCode = context.Message.FailedDescriptionCode!,
                 FailedDescriptionMessage = context.Message.FailedDescriptionMessage!
             }));

        return builder.Build();

        #endregion

    }
}

public class CreateSettlementChainTransactionConsumerDefinition : ConsumerDefinition<ProcessSettleRequestEventConsumer>
{
    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ProcessSettleRequestEventConsumer> consumerConfigurator,
        IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<SagaContext>(context);
    }
}