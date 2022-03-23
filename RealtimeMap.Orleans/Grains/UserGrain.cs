using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

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
        await _positionsStream.SubscribeAsync(OnPosition);
    }
    
    private async Task OnPosition(VehiclePosition position, StreamSequenceToken token)
    {
       
        if (_viewport is not null && position.IsWithinViewport(_viewport))
        {
            await _userPositionsStream!.OnNextAsync(position);
        }
    }
    
    public Task UpdateViewport(Viewport viewport)
    {
        _viewport = viewport;

        return Task.CompletedTask;
    }
    
    public async Task Deinitialize()
    {
        _viewport = null;
        
        foreach (var streamSubscriptionHandle in await _positionsStream!.GetAllSubscriptionHandles())
        {
            await streamSubscriptionHandle.UnsubscribeAsync();
        }
        
        DeactivateOnIdle();
    }
}