using Demo.Gateway.SMS.Infrastructure.MessageBroker;
using Demo.Gateway.SMS.Infrastructure.Telemetry;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services)
    =>
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

        services.AddMessageBus();
        services.AddTelemetry(hostContext.Configuration);
    });


var app = builder.Build();

app.Run();
