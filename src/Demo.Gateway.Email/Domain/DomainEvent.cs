using MediatR;

namespace Demo.Gateway.Email.Domain;

public abstract record DomainEvent : INotification
{
    public required Guid Id { get; init; }
}
