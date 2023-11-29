using System.Diagnostics;

namespace Demo.Api.Users.Infrastructure.Telemetry;

public static class TelemetryFactory
{
    public static ActivitySource CreateActivitySource()
        => new(TelemetrySetup.ServiceName, TelemetrySetup.ServiceVersion);
}
