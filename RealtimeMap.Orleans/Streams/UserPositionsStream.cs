using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Streams;

public static class UserPositionsStream
{
    public static readonly string Namespace = "UserPositions";

    public static IAsyncStream<VehiclePosition> GetUserPositionsStream(this IClusterClient client, Guid userId)
    {
        return client
            .GetMyStreamProvider()
            .GetUserPositionsStream(userId);
    }
    
    public static IAsyncStream<VehiclePosition> GetUserPositionsStream(this IStreamProvider streamProvider, Guid userId)
    {
        return streamProvider.GetStream<VehiclePosition>(userId, Namespace);
    }
}