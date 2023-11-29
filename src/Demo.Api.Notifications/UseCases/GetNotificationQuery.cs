using Demo.Api.Notifications.Domain;
using Demo.Api.Notifications.DTOs;
using Demo.Api.Notifications.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Notifications.UseCases;

public sealed record GetNotificationQuery(Guid Id) : IRequest<NotificationResponse>
{
    internal sealed class Handler(INotificationsRepository repository) : IRequestHandler<GetNotificationQuery, NotificationResponse>
    {
        private readonly INotificationsRepository _repository = repository;

        public async Task<NotificationResponse> Handle(GetNotificationQuery query, CancellationToken cancellationToken)
        {
            var notification = await _repository.GetAsync(query.Id, cancellationToken);
            if(notification is null)
            {
                throw new NotificationNotFoundException(query.Id);
            }

            NotificationResponse result = notification;

            return result;
        }
    }
}
