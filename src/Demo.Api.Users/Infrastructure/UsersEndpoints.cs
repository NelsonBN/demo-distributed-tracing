using Demo.Api.Users.Domain;
using Demo.Api.Users.DTOs;
using Demo.Api.Users.Infrastructure.Telemetry;
using Demo.Api.Users.UseCases;
using MediatR;

namespace Demo.Api.Users.Infrastructure;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints
            .MapGroup("/users")
            .WithOpenApi();


        group.MapGet("", async (IMediator mediator) =>
        {
            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Get: /users");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));

            var response = await mediator.Send(GetUsersQuery.Instance);

            return Results.Ok(response);
        });


        group.MapGet("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Get: /users/{id}");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
            activity!.SetTag("UserId", id.ToString());

            try
            {
                var response = await mediator.Send(new GetUserQuery(id));
                return Results.Ok(response);
            }
            catch(UserNotFoundException exception)
            {
                return Results.NotFound(exception.Message);
            }

        }).WithName("GetUser");


        group.MapPost("", async (IMediator mediator, UserRequest request) =>
        {
            var id = await mediator.Send(new CreateUserCommand(request));

            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Post: /users");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
            activity!.SetTag("UserId", id.ToString());

            return Results.CreatedAtRoute(
                "GetUser",
                new { id },
                id);
        });


        group.MapPut("{id:guid}", async (IMediator mediator, Guid id, UserRequest request) =>
        {
            try
            {
                await mediator.Send(new UpdateUserCommand(id, request));
            }
            catch(UserNotFoundException exception)
            {
                return Results.NotFound(exception.Message);
            }

            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Put: /users");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
            activity!.SetTag("UserId", id.ToString());

            return Results.NoContent();
        });


        group.MapDelete("{id:guid}", async (IMediator mediator, Guid id) =>
        {
            try
            {
                await mediator.Send(new DeleteUserCommand(id));
            }
            catch(UserNotFoundException exception)
            {
                return Results.NotFound(exception.Message);
            }

            var activitySource = TelemetryFactory.CreateActivitySource();
            using var activity = activitySource.StartActivity("Delete: /users");
            activity!.SetTag("At", DateTime.UtcNow.ToString("HH:mm:ss dd/MM/yyyy"));
            activity!.SetTag("UserId", id.ToString());

            return Results.NoContent();
        });
    }
}

