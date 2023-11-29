using Demo.Api.Notifications.Domain;

namespace Demo.Api.Notifications.DTOs;

public sealed record NotificationResponse(
    Guid Id,
    string Message,
    string? Email,
    NotificationStatus EmailNotificationStatus,
    string? Phone,
    NotificationStatus PhoneNotificationStatus)
{
    public static implicit operator NotificationResponse(Notification notification)
        => new(
            notification.Id,
            notification.Message,
            notification.Email,
            notification.EmailNotificationStatus,
            notification.Phone,
            notification.PhoneNotificationStatus);
}
