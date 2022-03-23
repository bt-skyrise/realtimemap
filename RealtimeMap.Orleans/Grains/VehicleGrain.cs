using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public class VehicleGrain : RealtimeMapGrain, IVehicleGrain
{
    private IAsyncStream<VehiclePosition>? _positionsStream;
    
    private readonly VehiclePositionHistory _vehiclePositionHistory = new();

    private string Id => this.GetPrimaryKeyString();

    public override Task OnActivateAsync()
    {
        _positionsStream = GetPositionsStream();
        
        return Task.CompletedTask;
    }

    public async Task OnPosition(VehiclePosition vehiclePosition)
    {
        _vehiclePositionHistory.Add(vehiclePosition);
        
        await _positionsStream!.OnNextAsync(vehiclePosition);

        await GrainFactory
            .GetGrain<IOrganizationGrain>(vehiclePosition.OrgId)
            .OnPosition(vehiclePosition);
    }

    public Task<VehiclePosition[]> GetPositionsHistory()
    {
        return Task.FromResult(_vehiclePositionHistory.Positions.ToArray());
    }
}