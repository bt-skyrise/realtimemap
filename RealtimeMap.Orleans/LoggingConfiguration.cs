using Serilog;

namespace RealtimeMap.Orleans;

public static class LoggingConfiguration
{
    public static void UseRealtimeMapLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, cfg)
            => cfg
                .ReadFrom.Configuration(context.Configuration)
                .Enrich.WithProperty("service", builder.Configuration["Service:Name"])
                .Enrich.WithProperty("env", builder.Environment.EnvironmentName)
        );
    }
}