using System.Text.Json.Serialization;
using Demo.Api.Notifications.Infrastructure;
using Demo.Api.Notifications.Infrastructure.Database;
using Demo.Api.Notifications.Infrastructure.MessageBroker;
using Demo.Api.Notifications.Infrastructure.Telemetry;
using Demo.Api.Notifications.Infrastructure.UsersApi;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddDatabase();
builder.Services.AddMessageBus();
builder.Services.AddUsersApi();

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddTelemetry(builder.Configuration);


var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.MapNotificationsEndpoints();

app.Run();
