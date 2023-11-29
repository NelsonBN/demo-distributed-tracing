using System.Runtime.InteropServices;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Demo.Gateway.SMS.Infrastructure.Telemetry;

public static class TelemetrySetup
{
    public static string ServiceName { get; }
    public static string ServiceVersion { get; }
    public static string ServiceFramework { get; }
    public static string Kernel { get; }

    static TelemetrySetup()
    {
        ServiceName = typeof(Program).Assembly.GetName().Name!;
        ServiceVersion = typeof(Program).Assembly.GetName().Version!.ToString();
        ServiceFramework = RuntimeInformation.FrameworkDescription;
        Kernel = Environment.OSVersion.VersionString;
    }

    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
            .WithTracing(traceProvider
                => traceProvider
                .AddSource(ServiceName)
                .SetResourceBuilder(
                    ResourceBuilder.CreateDefault()
                        .AddService(serviceName: ServiceName, serviceVersion: ServiceVersion))
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(opt =>
                {
                    opt.Endpoint = new Uri(configuration["TelemetryExporter"]!);
                    opt.Protocol = OtlpExportProtocol.Grpc;
                }));

        return services;
    }
}
