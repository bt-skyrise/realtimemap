using Orleans;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IVehicleGrain : IGrainWithStringKey
{
    Task OnPosition(VehiclePosition vehiclePosition);
    Task<VehiclePosition[]> GetPositionsHistory();
}