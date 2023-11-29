namespace Demo.Api.Notifications.DTOs;

public sealed record NotificationRequest(Guid UserId, string Message);
