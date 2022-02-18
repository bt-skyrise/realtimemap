using Orleans;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IOrganizationGrain : IGrainWithStringKey
{
    Task OnPosition(VehiclePosition vehiclePosition);
    Task<GeofenceDetails[]> GetGeofences();
}