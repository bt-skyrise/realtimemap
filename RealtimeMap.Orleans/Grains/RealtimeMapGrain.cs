using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;
using RealtimeMap.Orleans.Streams;

namespace RealtimeMap.Orleans.Grains;

public abstract class RealtimeMapGrain : Grain
{
    protected IAsyncStream<VehiclePosition> GetPositionsStream()
    {
        return this.GetStreamProvider(RealtimeMapSmsStreamProvider.Name)
            .GetPositionsStream();
    }
    
    protected IAsyncStream<VehiclePosition> GetUserPositionsStream(Guid userId)
    {
        return this.GetStreamProvider(RealtimeMapSmsStreamProvider.Name)
            .GetUserPositionsStream(userId);
    }
    
    protected IAsyncStream<Notification> GetNotificationsStream()
    {
        return this.GetStreamProvider(RealtimeMapSmsStreamProvider.Name)
            .GetNotificationsStream();
    }
}