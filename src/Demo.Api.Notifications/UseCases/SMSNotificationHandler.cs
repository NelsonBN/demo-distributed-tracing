using Demo.Api.Notifications.Domain;
using Demo.Api.Notifications.Domain.Events;
using Demo.Api.Notifications.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Notifications.UseCases;

public sealed class SMSNotificationHandler(ILogger<SMSNotificationHandler> Logger, INotificationsRepository Repository) :
    INotificationHandler<SMSNotificationSentEvent>,
    INotificationHandler<SMSNotificationFailedEvent>
{
    private readonly ILogger<SMSNotificationHandler> _logger = Logger;
    private readonly INotificationsRepository _repository = Repository;

    public async Task Handle(SMSNotificationSentEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.SMSSent();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }

    public async Task Handle(SMSNotificationFailedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.SMSFailed();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }
}
