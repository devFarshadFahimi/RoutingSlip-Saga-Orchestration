using MassTransit;
using SharedContracts.Events;
using WebApi.SagaOrchestration.DataLayer;

namespace WebApi.SagaOrchestration.StateMachines;

public class SettlementStateMachine : MassTransitStateMachine<SettlementState>
{
    public SettlementStateMachine()
    {
        InstanceState(x => x.CurrentState);
        ConfigureCorrelationIds();

        Initially(
            When(EventSettleRequestReceivedEvent)
                .Initialize()
                .InitiateProcessing()
                .TransitionTo(Received));

        During(Received,
            When(EventSettlementRequestSucceededEvent)
                .TransitionTo(Completed),
            When(EventSettlementRequestFailedEvent)
                .Then(context =>
                {
                    context.Saga.CorrelationId = context.Message.TrackingCode;
                    context.Saga.FailedDescriptionMessage = context.Message.FailedDescriptionMessage;
                    context.Saga.FailedDescriptionCode = context.Message.FailedDescriptionCode;
                })
               .Then(context => context.Publish(new ProcessSettleRequestFailedEvent()
               {
                   TrackingCode = context.Message.TrackingCode,
                   FailedDescriptionMessage = context.Message.FailedDescriptionMessage,
                   FailedDescriptionCode = context.Message.FailedDescriptionCode
               }))
                .TransitionTo(Failed));
    }

    #region Props
    public State Received { get; } = null!;
    public State Completed { get; } = null!;
    public State Failed { get; } = null!;

    public Event<SettleRequestReceivedEvent> EventSettleRequestReceivedEvent { get; } = null!;
    public Event<SettlementRequestSucceededEvent> EventSettlementRequestSucceededEvent { get; } = null!;
    public Event<SettlementRequestFailedEvent> EventSettlementRequestFailedEvent { get; } = null!;

    private void ConfigureCorrelationIds()
    {
        Event(() => EventSettleRequestReceivedEvent,
            x => x.CorrelateById(a => a.Message.TrackingCode));

        Event(() => EventSettlementRequestSucceededEvent,
            x => x.CorrelateById(a => a.Message.TrackingCode));

        Event(() => EventSettlementRequestFailedEvent,
            x => x.CorrelateById(a => a.Message.TrackingCode));
    }
    #endregion
}

static class SettlementStateMachineBehaviorExtensions
{
    public static EventActivityBinder<SettlementState, SettleRequestReceivedEvent> Initialize
        (this EventActivityBinder<SettlementState, SettleRequestReceivedEvent> binder)
    {
        return binder.Then(context =>
        {
            context.Saga.CorrelationId = context.Message.TrackingCode;
            context.Saga.Amount = context.Message.Amount;
        });
    }

    public static EventActivityBinder<SettlementState, SettleRequestReceivedEvent> InitiateProcessing(
        this EventActivityBinder<SettlementState, SettleRequestReceivedEvent> binder)
    {
        return binder.PublishAsync(context => context.Init<ProcessSettleRequestEvent>(
            new ProcessSettleRequestEvent()
            {
                TrackingCode = context.Message.TrackingCode,
                Amount = context.Message.Amount,
            }));
    }
}

public class SettlementStateMachineDefinition : SagaDefinition<SettlementState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<SettlementState> consumerConfigurator, IRegistrationContext context)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<SagaContext>(context);
        endpointConfigurator.UseDelayedRedelivery(r =>
         r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3)));
        endpointConfigurator.UseMessageRetry(r =>
            r.Intervals(TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(3)));
    }
}