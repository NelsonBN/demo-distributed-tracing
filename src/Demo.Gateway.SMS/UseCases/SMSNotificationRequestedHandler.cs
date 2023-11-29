using Demo.Gateway.SMS.Domain;
using Demo.Gateway.SMS.Infrastructure.MessageBroker;
using MediatR;

namespace Demo.Gateway.SMS.UseCases;

public sealed class SMSNotificationRequestedHandler(
    ILogger<SMSNotificationRequestedHandler> Logger,
    IMessageBus MessageBus)
: INotificationHandler<SMSNotificationRequestedEvent>
{
    private readonly ILogger<SMSNotificationRequestedHandler> _logger = Logger;
    private readonly IMessageBus _messageBus = MessageBus;

    public async Task Handle(SMSNotificationRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(task =>
            {
                if(delay % 6 == 0)
                {
                    _messageBus.Publish(new SMSNotificationFailedEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogWarning($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} failed");
                }

                else
                {
                    _messageBus.Publish(new SMSNotificationSentEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
                }
            }, cancellationToken);
    }
}
