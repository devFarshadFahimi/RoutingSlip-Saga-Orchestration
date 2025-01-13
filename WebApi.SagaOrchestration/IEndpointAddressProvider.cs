using MassTransit;

namespace WebApi.SagaOrchestration;

public interface IEndpointAddressProvider
{
    Uri GetExecuteEndpoint<T, TArguments>()
    where T : class, IExecuteActivity<TArguments>
    where TArguments : class;
}