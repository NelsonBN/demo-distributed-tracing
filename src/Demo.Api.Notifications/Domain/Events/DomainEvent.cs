using MediatR;

namespace Demo.Api.Notifications.Domain.Events;

public abstract record DomainEvent : INotification
{
    public required Guid Id { get; init; }
}
