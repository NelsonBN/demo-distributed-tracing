using System.Diagnostics;
using System.Text.Json;
using Demo.Gateway.Email.Domain;
using Demo.Gateway.Email.Infrastructure.Telemetry;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;


namespace Demo.Gateway.Email.Infrastructure.MessageBroker;

public interface IMessageBus
{
    void Publish<TMessage>(params TMessage[] domainEvents)
       where TMessage : DomainEvent;

    void Publish<TMessage>(TMessage domainEvent)
       where TMessage : DomainEvent;
}

internal sealed class MessageBus(
    ILogger<MessageBus> Logger,
    IOptions<MessageBusOptions> Options,
    IConnectionFactory Factory)
: IMessageBus
{
    private readonly ILogger<MessageBus> _logger = Logger;
    private readonly MessageBusOptions _options = Options.Value;
    private readonly IConnectionFactory _factory = Factory;

    public void Publish<TMessage>(params TMessage[] domainEvents)
        where TMessage : DomainEvent
    {
        foreach(var domainEvent in domainEvents)
        {
            Publish(domainEvent);
        }
    }

    public void Publish<TMessage>(TMessage domainEvent)
        where TMessage : DomainEvent
    {
        ArgumentNullException.ThrowIfNull(domainEvent);

        var connection = _factory.CreateConnection();
        var channel = connection.CreateModel();

        var messageType = domainEvent.GetType().Name;


        using var activity = TelemetryFactory.CreateActivitySource()
            .StartActivity($"Exchange {_options.ExchangeName} send", ActivityKind.Producer);

        var properties = channel.CreateBasicProperties()
            .SetAppId()
            .SetCorrelationId()
            .SetMessageId()
            .SetContentTypeJson()
            .SetEncodingUTF8()
            .SetMessageType(messageType);

        ActivityContext contextToInject = default;
        if(activity is not null)
        {
            contextToInject = activity.Context;
        }
        else if(Activity.Current is not null)
        {
            contextToInject = Activity.Current.Context;
        }

        Propagators.DefaultTextMapPropagator
            .Inject(new PropagationContext(contextToInject, Baggage.Current), properties, (IBasicProperties props, string key, string value) =>
            {
                try
                {
                    props.Headers ??= new Dictionary<string, object>();
                    props.Headers[key] = value;
                }
                catch(Exception exception)
                {
                    _logger.LogError(exception, "[MESSAGE BUS][PUBLISHER] Failed to inject trace context");
                }
            });


        activity?.SetTag("messaging.system", "rabbitmq");
        activity?.SetTag("messaging.destination_kind", "exchange");
        activity?.SetTag("messaging.destination", _options.ExchangeName);
        activity?.SetTag("messaging.rabbitmq.routing_key", messageType);
        activity?.SetTag("message", JsonSerializer.Serialize(domainEvent));


        channel.BasicPublish(
            exchange: _options.ExchangeName,
            routingKey: messageType,
            basicProperties: properties,
            body: domainEvent.Serialize());

        _logger.LogInformation("[MESSAGE BUS][PUBLISHER] {MessageType} published", messageType);


        channel.Close();
        connection.Close();
    }
}
