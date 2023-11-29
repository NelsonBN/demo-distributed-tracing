using Demo.Api.Users.Infrastructure;
using Demo.Api.Users.Infrastructure.Database;
using Demo.Api.Users.Infrastructure.Telemetry;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());

builder.Services.AddDatabase();


builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

builder.Services.AddTelemetry(builder.Configuration);


var app = builder.Build();

app.UseSwagger()
   .UseSwaggerUI();

app.MapUsersEndpoints();

app.Run();
