using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IUserGrain : IGrainWithGuidKey
{
    Task Initialize();
    Task UpdateViewport(Viewport viewport);
    Task Poison();
}

public class UserGrain : RealtimeMapGrain, IUserGrain
{
    private IAsyncStream<VehiclePosition>? _positionsStream;
    private IAsyncStream<VehiclePosition>? _userPositionsStream;
    
    private Viewport? _viewport;

    private Guid Id => this.GetPrimaryKey();

    public override async Task OnActivateAsync()
    {
        _positionsStream = GetPositionsStream();
        _userPositionsStream = GetUserPositionsStream(Id);

        foreach (var streamSubscriptionHandle in await _positionsStream.GetAllSubscriptionHandles())
        {
            await streamSubscriptionHandle.ResumeAsync(OnPosition);
        }
    }

    public async Task Initialize()
    {
        var subscriptionHandles = await _positionsStream!.GetAllSubscriptionHandles();

        if (!subscriptionHandles.Any())
        {
            await _positionsStream.SubscribeAsync(OnPosition);
        }
    }
    
    public Task UpdateViewport(Viewport viewport)
    {
        _viewport = viewport;

        return Task.CompletedTask;
    }
    
    public Task Poison()
    {
        DeactivateOnIdle();
        return Task.CompletedTask;
    }

    private async Task OnPosition(VehiclePosition position, StreamSequenceToken token)
    {
        Console.WriteLine($"User {Id}: received {position}");
        
        if (_viewport is not null && position.IsWithinViewport(_viewport))
        {
            await _userPositionsStream!.OnNextAsync(position);
        }
    }
}