using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Demo.Gateway.Email.Domain;
using Demo.Gateway.Email.Infrastructure.Telemetry;
using MediatR;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Demo.Gateway.Email.Infrastructure.MessageBroker;

internal sealed class MessageBusConsumer(
    ILogger<MessageBusConsumer> Logger,
    IOptions<MessageBusOptions> Options,
    IConnectionFactory Factory,
    IServiceProvider ServiceProvider)
: BackgroundService
{
    private readonly ILogger<MessageBusConsumer> _logger = Logger;
    private readonly MessageBusOptions _options = Options.Value;
    private readonly IConnectionFactory _factory = Factory;
    private readonly IServiceProvider _serviceProvider = ServiceProvider;

    private AsyncEventingBasicConsumer _consumer = default!;
    private IModel _channel = default!;

    private static readonly Dictionary<string, Type> _supportedMessages;

    static MessageBusConsumer()
        => _supportedMessages = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(t => !t.IsAbstract)
            .Where(t => t.IsAssignableTo(typeof(DomainEvent)))
            .ToDictionary(t => t.Name, t => t);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[MESSAGE BUS][WORKER][CONSUMER] starting...");

        var connection = _factory.CreateConnection();
        _channel = connection.CreateModel();

        _channel.BasicQos(
            0,
            1, // Total message receive same time
            false); // [ false per consumer | true per channel ]


        _consumer = new AsyncEventingBasicConsumer(_channel);
        _consumer.Received += (_, arg) => _listener(arg);

        _channel.BasicConsume(
            queue: _options.QueueName,
            autoAck: false,
            consumer: _consumer);

        _logger.LogInformation("[MESSAGE BUS][WORKER][CONSUMER] started");



        // Keep worker running
        while(!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER] Worker running at: {DateTimeOffset.UtcNow}");
            await Task.Delay(5000, stoppingToken);
        }
    }


    private async Task _listener(BasicDeliverEventArgs args)
    {
        try
        {
            var parentContext = Propagators.DefaultTextMapPropagator.Extract(default, args.BasicProperties, (IBasicProperties props, string key) =>
            {
                try
                {
                    if(props.Headers.TryGetValue(key, out var value))
                    {
                        var bytes = value as byte[];
                        return new[] { Encoding.UTF8.GetString(bytes!) };
                    }
                }
                catch(Exception exception)
                {
                    _logger.LogError(exception, "[MESSAGE BUS][WORKER][CONSUMER] Error during extract trace context");
                }

                return Enumerable.Empty<string>();
            });
            Baggage.Current = parentContext.Baggage;



            var domainEvent = _deserialize(args);


            using var activity = TelemetryFactory.CreateActivitySource()
                .StartActivity($"Consumer {args.RoutingKey} receive", ActivityKind.Consumer, parentContext.ActivityContext);

            activity?.SetTag("messaging.system", "rabbitmq");
            activity?.SetTag("messaging.destination_kind", "queue");
            activity?.SetTag("messaging.destination", _options.ExchangeName);
            activity?.SetTag("messaging.rabbitmq.routing_key", args.RoutingKey);
            activity?.SetTag("message", JsonSerializer.Serialize(domainEvent));


            await _handle(domainEvent);

            _channel.BasicAck(
                args.DeliveryTag,
                false);
        }
        catch(NotSupportedException exception)
        {
            _logger.LogError(
               exception,
               $"[MESSAGE BUS][CONSUMER][DESERIALIZE]");

            _channel.BasicReject(
                args.DeliveryTag,
                false);
        }
        catch(Exception exception)
        {
            _logger.LogError(
                exception,
                $"[MESSAGE BUS][CONSUMER][HANDLE]");

            _channel.BasicNack(
                args.DeliveryTag,
                false,
                false);
        }
    }

    private async Task _handle(DomainEvent domainEvent)
    {
        using var scope = _serviceProvider.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Publish(domainEvent);
    }

    private static DomainEvent _deserialize(BasicDeliverEventArgs args)
    {
        var messageType = args.GetMessageType();

        if(_supportedMessages.TryGetValue(messageType, out var type))
        {
            if(args.Deserialize(type) is not DomainEvent message)
            {
                throw new NotSupportedException($"The message type '{messageType}' is null");
            }

            return message;
        }

        throw new NotSupportedException($"The message type '{messageType}' is not supported by consummer");
    }
}
