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
            .AddStreaming()
            .AddSimpleMessageStreamProvider(RealtimeMapSmsStreamProvider.Name)
            .AddMemoryGrainStorage("PubSubStore")
            .Configure<ClusterOptions>(options =>
            {
                options.ClusterId = "dev";
                options.ServiceId = "OrleansBasics";
            })
            .Configure<StatisticsOptions>(options =>
            {
                options.LogWriteInterval = Timeout.InfiniteTimeSpan;
            })
            .ConfigureLogging(logging => logging
                .AddSerilog()
            )
            .Configure<GrainTypeOptions>(options =>
            {
                options.Interfaces.Add(typeof(IGeofenceGrain));
                options.Classes.Add(typeof(GeofenceGrain));
                
                options.Interfaces.Add(typeof(IOrganizationGrain));
                options.Classes.Add(typeof(OrganizationGrain));

                options.Interfaces.Add(typeof(IUserGrain));
                options.Classes.Add(typeof(UserGrain));

                options.Interfaces.Add(typeof(IVehicleGrain));
                options.Classes.Add(typeof(VehicleGrain));
            })
            // .ConfigureApplicationParts(parts => parts
            //     .AddApplicationPart(typeof(VehicleGrain).Assembly)
            //     .WithReferences()
            // )
        );
    }
}