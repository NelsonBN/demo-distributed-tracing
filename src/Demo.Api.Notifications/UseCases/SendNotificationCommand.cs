using Demo.Api.Notifications.Domain;
using Demo.Api.Notifications.DTOs;
using Demo.Api.Notifications.Infrastructure.Database;
using Demo.Api.Notifications.Infrastructure.MessageBroker;
using Demo.Api.Notifications.Infrastructure.UsersApi;
using MediatR;

namespace Demo.Api.Notifications.UseCases;

public sealed record SendNotificationCommand(NotificationRequest Request) : IRequest<Guid>
{
    internal sealed class Handler(IUsersApiService UserApi, INotificationsRepository Repository, IMessageBus MessageBus) : IRequestHandler<SendNotificationCommand, Guid>
    {
        private readonly IUsersApiService _userApi = UserApi;
        private readonly INotificationsRepository _repository = Repository;
        private readonly IMessageBus _messageBus = MessageBus;

        public async Task<Guid> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
        {
            var user = await _userApi.GetUserAsync(command.Request.UserId, cancellationToken);
            if(user is null)
            {
                throw new UserNotFoundException(command.Request.UserId);
            }

            var notification = Notification.Create(
                command.Request.Message,
                user.Email,
                user.Phone);

            await _repository.AddAsync(notification, cancellationToken);

            _messageBus.Publish(notification.GetDomainEvents());

            return notification.Id;
        }
    }
}
