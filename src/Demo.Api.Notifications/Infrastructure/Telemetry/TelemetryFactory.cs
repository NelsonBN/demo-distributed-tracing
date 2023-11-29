using System.Diagnostics;

namespace Demo.Api.Notifications.Infrastructure.Telemetry;

public static class TelemetryFactory
{
    public static ActivitySource CreateActivitySource()
        => new(TelemetrySetup.ServiceName, TelemetrySetup.ServiceVersion);
}
