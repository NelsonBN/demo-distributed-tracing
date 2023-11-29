using Demo.Gateway.Email.Domain;
using Demo.Gateway.Email.Infrastructure.MessageBroker;
using MediatR;

namespace Demo.Gateway.Email.UseCases;

public sealed class EmailNotificationRequestedHandler(
    ILogger<EmailNotificationRequestedHandler> Logger,
    IMessageBus MessageBus)
: INotificationHandler<EmailNotificationRequestedEvent>
{
    private readonly ILogger<EmailNotificationRequestedHandler> _logger = Logger;
    private readonly IMessageBus _messageBus = MessageBus;

    public async Task Handle(EmailNotificationRequestedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var delay = Random.Shared.Next(50, 1000);

        await Task.Delay(delay, cancellationToken)
            .ContinueWith(task =>
            {
                if(delay % 6 == 0)
                {
                    _messageBus.Publish(new EmailNotificationFailedEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogWarning($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} failed");
                }

                else
                {
                    _messageBus.Publish(new EmailNotificationSentEvent
                    {
                        Id = domainEvent.Id,
                    });

                    _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
                }

            }, cancellationToken);
    }
}
