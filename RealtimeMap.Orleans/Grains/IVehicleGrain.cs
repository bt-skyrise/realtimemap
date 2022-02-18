using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IVehicleGrain : IGrainWithStringKey
{
    Task OnPosition(VehiclePosition vehiclePosition);
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

    public async Task OnPosition(VehiclePosition vehiclePosition)
    {
        Console.WriteLine($"Vehicle {Id}: received {vehiclePosition}");
        
        _currentPosition = vehiclePosition;

        await _positionsStream!.OnNextAsync(vehiclePosition);

        await GrainFactory
            .GetGrain<IOrganizationGrain>(vehiclePosition.OrgId)
            .OnPosition(vehiclePosition);
    }
}