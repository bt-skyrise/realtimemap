using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Hubs;

public class EventsHub : Hub
{
    private readonly IHubContext<EventsHub> _eventsHubContext;
    private readonly ILogger<EventsHub> _logger;

    public EventsHub(IHubContext<EventsHub> eventsHubContext, ILogger<EventsHub> logger)
    {
        _logger = logger;

        // since the Hub is scoped per request, we need the IHubContext to be able to
        // push messages from the User actor
        _eventsHubContext = eventsHubContext;
    }

    // private PID UserActorPid
    // {
    //     get => Context.Items["user-pid"] as PID;
    //     set => Context.Items["user-pid"] = value;
    // }

    public override Task OnConnectedAsync()
    {
        // initialize user grain
        // subscribe to user's stream (with generated guid?)
        
        _logger.LogInformation("Client {ClientId} connected", Context.ConnectionId);
        
        var connectionId = Context.ConnectionId;
        
        // UserActorPid = _cluster.System.Root.Spawn(
        //     Props.FromProducer(() => new UserActor(
        //             batch => SendPositionBatch(connectionId, batch),
        //             notification => SendNotification(connectionId, notification)
        //         ))
        //         .WithTracing()
        // );

        return Task.CompletedTask;
    }

    public Task SetViewport(double swLng, double swLat, double neLng, double neLat)
    {
        // update user grain's viewport
        
        _logger.LogInformation("Client {ClientId} setting viewport to ({SWLat}, {SWLng}),({NELat}, {NELng})",
            Context.ConnectionId, swLat, swLng, neLat, neLng);

        // _senderContext.Send(UserActorPid, new UpdateViewport
        // {
        //     Viewport = new Viewport
        //     {
        //         SouthWest = new GeoPoint(swLng, swLat),
        //         NorthEast = new GeoPoint(neLng, neLat)
        //     }
        // });

        return Task.CompletedTask;
    }

    private async Task SendPositionBatch(string connectionId, GeoPoint[] batch)
    {
        // await _eventsHubContext.Clients.Client(connectionId).SendAsync("positions",
        //     new PositionsDto
        //     {
        //         Positions = batch.Positions
        //             .Select(PositionDto.MapFrom)
        //             .ToArray()
        //     });
    }

    // private async Task SendNotification(string connectionId)
    // {
    //     try
    //     {
    //         await _eventsHubContext.Clients.Client(connectionId)
    //             .SendAsync("notification", NotificationDto.MapFrom(notification));
    //     }
    //     catch (Exception e)
    //     {
    //         activity?.RecordException(e);
    //         activity?.SetStatus(Status.Error);
    //         throw;
    //     }
    // }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        // unsubscribe viewport, kill grain
        
        // _logger.LogDebug("Client {ClientId} disconnected", Context.ConnectionId);
        // RealtimeMapMetrics.SignalRConnections.ChangeBy(-1);
        //
        // await _cluster.System.Root.StopAsync(UserActorPid);
    }
}