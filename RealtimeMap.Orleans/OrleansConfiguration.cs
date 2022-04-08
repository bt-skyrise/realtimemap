using Orleans;
using Orleans.Configuration;
using Orleans.Hosting;
using Orleans.Providers.MongoDB.Configuration;
using Orleans.Statistics;
using RealtimeMap.Orleans.Grains;
using RealtimeMap.Orleans.Streams;
using Serilog;

namespace RealtimeMap.Orleans;

public static class OrleansConfiguration
{
    public static void UseRealtimeMapOrleans(this WebApplicationBuilder builder)
    {
        //var primarySiloEndpoint = new IPEndPoint(IPAddress.Loopback, 11111);
        
        builder.Host.UseOrleans(siloBuilder => siloBuilder
            
            // LOCALHOST CLUSTERING
            // .UseLocalhostClustering(
            //     primarySiloEndpoint: primarySiloEndpoint,
            //     siloPort: builder.Configuration.GetValue("SiloPort", 11111),
            //     gatewayPort: builder.Configuration.GetValue("GatewayPort", 30000))
            
            
            // MOGODB CLUSTERING
            .ConfigureEndpoints(siloPort: builder.Configuration.GetValue("SiloPort", 11111), gatewayPort: builder.Configuration.GetValue("GatewayPort", 30000))
            .UseMongoDBClient("mongodb://mongoadmin:secret@localhost:27017/test?authSource=admin&directConnection=true&ssl=false")
            .UseMongoDBClustering(opt =>
            {
                opt.DatabaseName = "membership";
                opt.Strategy = MongoDBMembershipStrategy.SingleDocument;
            })
            
            
            
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
            .UsePerfCounterEnvironmentStatistics()
            .UseDashboard(opt =>
            {
                opt.Port = builder.Configuration.GetValue("DashboardPort", 8001);
            })
        );
    }

}