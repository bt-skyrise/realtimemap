using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using RealtimeMap.Orleans.Grains;
using RealtimeMap.Orleans.Streams;
using Serilog;

namespace RealtimeMap.Orleans;

public static class OrleansConfiguration
{
    public static void UseRealtimeMapOrleans(this WebApplicationBuilder builder)
    {
        builder.Host.UseOrleans(siloBuilder => siloBuilder
            .UseLocalhostClustering()
            .AddSimpleMessageStreamProvider(RealtimeMapSmsStreamProvider.Name)
            .AddMemoryGrainStorage("PubSubStore")
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "OrleansBasics";
            })
            .Configure<StatisticsOptions>(options => { options.LogWriteInterval = Timeout.InfiniteTimeSpan; })
            .ConfigureApplicationParts(parts => parts
                .AddApplicationPart(typeof(VehicleGrain).Assembly)
                .WithReferences()
            )
            .ConfigureLogging(logging => logging
                .AddSerilog()
            )
        );
    }
}