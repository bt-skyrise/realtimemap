using RealtimeMap.Ingress;
using RealtimeMap.Orleans.Positions;
using Serilog;
using ILogger = Serilog.ILogger;

namespace RealtimeMap.Orleans;

public class IngressHostedService : IHostedService
{
    private static ILogger Logger = Log.ForContext<IngressHostedService>();
    
    private readonly IConfiguration _configuration;
    private readonly ILoggerFactory _loggerFactory;
    private HrtPositionsSubscription _hrtPositionsSubscription;

    public IngressHostedService(IConfiguration configuration, ILoggerFactory loggerFactory)
    {
        _configuration = configuration;
        _loggerFactory = loggerFactory;
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

        var geoPoint = new GeoPoint(
            hrtPositionUpdate.VehiclePosition.Long.GetValueOrDefault(0),
            hrtPositionUpdate.VehiclePosition.Lat.GetValueOrDefault(0)
        );
        
        var vehiclePosition = new VehiclePosition(
            OrgId: hrtPositionUpdate.OperatorId,
            Position: geoPoint,
            VehicleId: vehicleId,
            Timestamp: hrtPositionUpdate.VehiclePosition.Tst.GetValueOrDefault().DateTime
        );
        
        Logger.Information("Received position: {Position}.", vehiclePosition);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _hrtPositionsSubscription?.Dispose();
        return Task.CompletedTask;
    }
}