using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Streams;

public static class PositionsStream
{
    public static readonly string Namespace = "Positions";
    public static readonly Guid Id = new("49ceb72e-779c-4da3-b276-389a9369bd61");
    
    public static IAsyncStream<VehiclePosition> GetPositionsStream(this IStreamProvider streamProvider)
    {
        return streamProvider.GetStream<VehiclePosition>(Id, Namespace);
    }
}