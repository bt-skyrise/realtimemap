using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IVehicleGrain : IGrainWithStringKey
{
    Task OnPosition(VehiclePosition position);
}

public class VehicleGrain : RealtimeMapGrain, IVehicleGrain
{
    private IAsyncStream<VehiclePosition>? _positionsStream;
    
    private VehiclePosition? _currentPosition;
    
    private string Id => this.GetPrimaryKeyString();

    public override Task OnActivateAsync()
    {
        _positionsStream = GetPositionsStream();
        
        return Task.CompletedTask;
    }

    public Task OnPosition(VehiclePosition position)
    {
        Console.WriteLine($"Vehicle {Id}: received {position}");
        
        _currentPosition = position;

        _positionsStream!.OnNextAsync(position);

        return Task.CompletedTask;
    }
}