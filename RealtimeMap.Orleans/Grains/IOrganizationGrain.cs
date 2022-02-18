using Orleans;

namespace RealtimeMap.Orleans.Grains;

public interface IOrganizationGrain : IGrainWithStringKey
{
    Task<GeofenceDetails[]> GetGeofences();
}