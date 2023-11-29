using Demo.Api.Notifications.DTOs;
using Demo.Api.Notifications.Infrastructure.Database;
using MediatR;

namespace Demo.Api.Notifications.UseCases;

public sealed record GetNotificationsQuery : IRequest<IEnumerable<NotificationResponse>>
{
    public static GetNotificationsQuery Instance => new();

    internal sealed class Handler(INotificationsRepository repository) : IRequestHandler<GetNotificationsQuery, IEnumerable<NotificationResponse>>
    {
        private readonly INotificationsRepository _repository = repository;

        public async Task<IEnumerable<NotificationResponse>> Handle(GetNotificationsQuery query, CancellationToken cancellationToken)
        {
            var notifications = await _repository.ListAsync(cancellationToken);

            var result = notifications
                .Select(n => (NotificationResponse)n);

            return result;
        }
    }
}
