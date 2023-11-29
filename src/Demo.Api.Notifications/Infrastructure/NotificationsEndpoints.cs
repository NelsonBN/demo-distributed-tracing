using Demo.Api.Notifications.Domain;
using Demo.Api.Notifications.DTOs;
using Demo.Api.Notifications.Infrastructure.Telemetry;
using Demo.Api.Notifications.UseCases;
using MediatR;

namespace Demo.Api.Notifications.Infrastructure;

public static class NotificationsEndpoints
{
    public static void MapNotificationsEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/notifications")
            .WithOpenApi();


        group.MapGet("", async (IMediator mediator) =>
        {
            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Get: /notifications");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));

            var response = await mediator.Send(GetNotificationsQuery.Instance);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Get: /notifications/{id}");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
            activity!.SetTag("NotificationId", id.ToString());

            try
            {
                var response = await mediator.Send(new GetNotificationQuery(id));
                return Results.Ok(response);
            }
            catch(NotificationNotFoundException exception)
            {
                return Results.NotFound(exception.Message);
            }

        }).WithName("GetNotification");



        group.MapPost("", async (IMediator mediator, NotificationRequest request) =>
        {
            try
            {
                var id = await mediator.Send(new SendNotificationCommand(request));

                var activitySource = TelemetryFactory.CreateActivitySource();
                using var activity = activitySource.StartActivity("Post: /notifications");
                activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
                activity!.SetTag("NotificationId", id.ToString());
                activity!.SetTag("UserId", request.UserId.ToString());

                return Results.AcceptedAtRoute(
                    "GetNotification",
                    new { id },
                    id);
            }
            catch(UserNotFoundException exception)
            {
                return Results.NotFound(exception.Message);
            }
        });
    }
}
