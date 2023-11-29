using System.Diagnostics;

namespace Demo.Gateway.Email.Infrastructure.Telemetry;

public static class TelemetryFactory
{
    public static ActivitySource CreateActivitySource()
        => new(TelemetrySetup.ServiceName, TelemetrySetup.ServiceVersion);
}
