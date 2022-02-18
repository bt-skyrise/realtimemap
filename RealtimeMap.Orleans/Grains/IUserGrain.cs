using Orleans;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IUserGrain : IGrainWithGuidKey
{
    Task Initialize();
    Task UpdateViewport(Viewport viewport);
    Task Deinitialize();
}