using MassTransit;

namespace SharedContracts.Services;
public class IntegrationBus : IIntegrationBus
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public IntegrationBus(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish<TEvent>(TEvent @event) where TEvent : class
    {
        await _publishEndpoint.Publish(@event);
    }

    public async Task Send<TCommand>(TCommand command) where TCommand : class
    {
        var uri = QueueNames.GetMessageUri(typeof(TCommand).Name);
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(
            uri);

        await endpoint.Send(command);
    }
}

public interface IIntegrationBus
{
    Task Publish<TEvent>(TEvent @event) where TEvent : class;
    Task Send<TCommand>(TCommand command) where TCommand : class;
}
