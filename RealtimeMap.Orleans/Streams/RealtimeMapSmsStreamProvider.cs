using Orleans;
using Orleans.Streams;

namespace RealtimeMap.Orleans.Streams;

public static class RealtimeMapSmsStreamProvider
{
    public const string Name = "SMSProvider";

    public static IStreamProvider GetMyStreamProvider(this IClusterClient client)
    {
        return client.GetStreamProvider(Name);
    }
}