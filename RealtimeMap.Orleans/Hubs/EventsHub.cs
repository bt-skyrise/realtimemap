using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.DTO;
using RealtimeMap.Orleans.Grains;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;
using RealtimeMap.Orleans.Streams;

namespace RealtimeMap.Orleans.Hubs;

public class EventsHub : Hub
{
    private readonly IHubContext<EventsHub> _eventsHubContext;
    private readonly ILogger<EventsHub> _logger;
    private readonly IClusterClient _clusterClient;

    public EventsHub(IHubContext<EventsHub> eventsHubContext, ILogger<EventsHub> logger, IClusterClient clusterClient)
    {
        _logger = logger;
        _clusterClient = clusterClient;

        // since the Hub is scoped per request, we need the IHubContext to be able to
        // push messages from the User actor
        _eventsHubContext = eventsHubContext;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogInformation("Client {ClientId} connected", Context.ConnectionId);

        UserId = Guid.NewGuid();
        
        var connectionId = Context.ConnectionId;

        var userGrain = GetUserGrain();

        await userGrain.Initialize();
        
        VehiclePositionsSubscription = await _clusterClient
            .GetUserPositionsStream(UserId)
            .SubscribeAsync(async (position, _) =>
            {
                await SendPositionBatch(connectionId, new[] { position });
            });

        NotificationsSubscription = await _clusterClient
            .GetNotificationsStream()
            .SubscribeAsync(async (notification, _) =>
            {
                await SendNotification(connectionId, notification);
            });
    }
    
    public async Task SetViewport(double swLng, double swLat, double neLng, double neLat)
    {
        _logger.LogInformation("Client {ClientId} setting viewport to ({SWLat}, {SWLng}),({NELat}, {NELng})",
            Context.ConnectionId, swLat, swLng, neLat, neLng);
        
        var userGrain = _clusterClient.GetGrain<IUserGrain>(UserId);

        await userGrain.UpdateViewport(new Viewport(
            SouthWest: new GeoPoint(swLng, swLat),
            NorthEast: new GeoPoint(neLng, neLat)
        ));
    }

    private async Task SendPositionBatch(string connectionId, VehiclePosition[] batch)
    {
        await _eventsHubContext.Clients.Client(connectionId).SendAsync("positions",
            new PositionsDto
            {
                Positions = batch
                    .Select(PositionDto.MapFrom)
                    .ToArray()
            }
        );
    }

    private async Task SendNotification(string connectionId, Notification notification)
    {
        await _eventsHubContext.Clients
            .Client(connectionId)
            .SendAsync("notification", NotificationDto.MapFrom(notification));
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userGrain = GetUserGrain();

        await userGrain.Deinitialize();

        if (VehiclePositionsSubscription is not null)
        {
            await VehiclePositionsSubscription.UnsubscribeAsync();
        }

        if (NotificationsSubscription is not null)
        {
            await NotificationsSubscription.UnsubscribeAsync();
        }
    }
    
    private IUserGrain GetUserGrain()
    {
        return _clusterClient.GetGrain<IUserGrain>(UserId);
    }
    
    private Guid UserId
    {
        get => (Guid?)Context.Items[nameof(UserId)] ?? throw new InvalidOperationException("User ID not set.");
        set => Context.Items[nameof(UserId)] = value;
    }

    private StreamSubscriptionHandle<VehiclePosition>? VehiclePositionsSubscription
    {
        get => (StreamSubscriptionHandle<VehiclePosition>?)Context.Items[nameof(VehiclePositionsSubscription)];
        set => Context.Items[nameof(VehiclePositionsSubscription)] = value;
    }
    
    private StreamSubscriptionHandle<Notification>? NotificationsSubscription
    {
        get => (StreamSubscriptionHandle<Notification>?)Context.Items[nameof(NotificationsSubscription)];
        set => Context.Items[nameof(NotificationsSubscription)] = value;
    }
}