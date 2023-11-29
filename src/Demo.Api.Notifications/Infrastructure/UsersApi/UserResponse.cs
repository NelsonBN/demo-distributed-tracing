namespace Demo.Api.Notifications.Infrastructure.UsersApi;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone);
