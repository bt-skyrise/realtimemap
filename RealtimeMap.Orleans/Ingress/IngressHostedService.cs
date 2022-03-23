using Orleans;
using RealtimeMap.Ingress;
using RealtimeMap.Orleans.Grains;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace RealtimeMap.Orleans.Ingress;

public class IngressHostedService : IHostedService
{
    private static readonly ILogger Logger = Log.ForContext<IngressHostedService>();
    
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IClusterClient _client;
    
    private HrtPositionsSubscription? _hrtPositionsSubscription;

    public IngressHostedService(IConfiguration configuration, ILoggerFactory loggerFactory, IClusterClient client)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _hrtPositionsSubscription = await HrtPositionsSubscription.Start(
            sharedSubscriptionGroupName: GetSharedSubscriptionGroupName(),
            onPositionUpdate: ProcessHrtPositionUpdate,
            loggerFactory: _loggerFactory,
            cancel: cancellationToken
        );
    }

    private string GetSharedSubscriptionGroupName()
    {
        var sharedSubscriptionGroupName = _configuration["RealtimeMap:SharedSubscriptionGroupName"];

        return string.IsNullOrEmpty(sharedSubscriptionGroupName)
            ? $"group-{Guid.NewGuid()}"
            : sharedSubscriptionGroupName;
    }

    private async Task ProcessHrtPositionUpdate(HrtPositionUpdate hrtPositionUpdate)
    {
        var vehicleId = $"{hrtPositionUpdate.OperatorId}.{hrtPositionUpdate.VehicleNumber}";

        var position = new GeoPoint(
            hrtPositionUpdate.VehiclePosition.Long.GetValueOrDefault(),
            hrtPositionUpdate.VehiclePosition.Lat.GetValueOrDefault()
        );
        
        var vehiclePosition = new VehiclePosition(
            OrgId: hrtPositionUpdate.OperatorId,
            Position: position,
            VehicleId: vehicleId,
            Timestamp: hrtPositionUpdate.VehiclePosition.Tst.GetValueOrDefault().DateTime,
            Heading: (int)hrtPositionUpdate.VehiclePosition.Hdg.GetValueOrDefault(),
            DoorsOpen: hrtPositionUpdate.VehiclePosition.Drst == 1,
            Speed: hrtPositionUpdate.VehiclePosition.Spd.GetValueOrDefault()
        );

        var vehicleGrain = _client.GetGrain<IVehicleGrain>(vehicleId);
        
        await vehicleGrain.OnPosition(vehiclePosition);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _hrtPositionsSubscription?.Dispose();
        return Task.CompletedTask;
    }
}