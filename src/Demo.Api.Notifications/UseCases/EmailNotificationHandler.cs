using Demo.Api.Notifications.Domain;
using Demo.Api.Notifications.Domain.Events;
using Demo.Api.Notifications.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Notifications.UseCases;

public sealed class EmailNotificationHandler(ILogger<EmailNotificationHandler> Logger, INotificationsRepository Repository) :
    INotificationHandler<EmailNotificationSentEvent>,
    INotificationHandler<EmailNotificationFailedEvent>
{
    private readonly ILogger<EmailNotificationHandler> _logger = Logger;
    private readonly INotificationsRepository _repository = Repository;

    public async Task Handle(EmailNotificationSentEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.EmailSent();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }

    public async Task Handle(EmailNotificationFailedEvent domainEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} received");

        var notification = await _repository.GetAsync(domainEvent.Id, cancellationToken);
        if(notification is null)
        {
            throw new NotificationNotFoundException(domainEvent.Id);
        }

        notification.EmailFailed();

        await _repository.UpdateAsync(notification, cancellationToken);

        _logger.LogInformation($"[MESSAGE BUS][WORKER][CONSUMER][HANDLER] {domainEvent.GetType().Name} handled");
    }

}
