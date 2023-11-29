using MediatR;

namespace Demo.Gateway.SMS.Domain;

public abstract record DomainEvent : INotification
{
    public required Guid Id { get; init; }
}
