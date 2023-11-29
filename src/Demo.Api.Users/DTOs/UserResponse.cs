using Demo.Api.Users.Domain;

namespace Demo.Api.Users.DTOs;

public sealed record UserResponse(
    Guid Id,
    string Name,
    string? Email,
    string? Phone)
{
    public static implicit operator UserResponse(User user)
        => new(
            user.Id,
            user.Name,
            user.Email,
            user.Phone);
}
