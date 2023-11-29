using Demo.Gateway.SMS.Domain;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Demo.Gateway.SMS.Infrastructure.MessageBroker;

internal sealed class MessageBusSetupWorker(
    IOptions<MessageBusOptions> Options,
    ILogger<MessageBusSetupWorker> Logger,
    IConnectionFactory Factory)
: BackgroundService
{
    private readonly MessageBusOptions _options = Options.Value;
    private readonly ILogger<MessageBusSetupWorker> _logger = Logger;
    private readonly IConnectionFactory _factory = Factory;


    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("[MESSAGE BUS][SETUP][WORKER] Setup starting...");

        try
        {
            var connection = _factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(
                exchange: _options.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null);

            channel.QueueDeclare(
                queue: _options.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(
                queue: _options.QueueName,
                exchange: _options.ExchangeName,
                routingKey: typeof(SMSNotificationRequestedEvent).Name);

            _logger.LogInformation("[MESSAGE BUS][SETUP][WORKER] Finished setup");

            channel.Close();
            connection.Close();
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "[MESSAGE BUS][SETUP][WORKER] Error while creating exchange");
        }

        return Task.CompletedTask;
    }
}
