using Orleans;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public class OrganizationGrain : RealtimeMapGrain, IOrganizationGrain
{
    private Organization? _organization;
    private IGeofenceGrain[]? _geofenceGrains;
    
    private string Id => this.GetPrimaryKeyString();

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _organization = Organizations.ById[Id];

        _geofenceGrains = _organization.Geofences
            .Select(geofence => GrainFactory.GetGeofenceGrain(Id, geofence.Name))
            .ToArray();
        
        return Task.CompletedTask;
    }

    public async Task OnPosition(VehiclePosition vehiclePosition)
    {
        if (_organization is null || _geofenceGrains is null)
        {
            throw new InvalidOperationException($"Organization {Id} doesn't exist.");
        }
        
        var tasks = _geofenceGrains
            .Select(geofence => geofence.OnPosition(vehiclePosition))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    public async Task<GeofenceDetails[]> GetGeofences()
    {
        if (_organization is null || _geofenceGrains is null)
        {
            throw new InvalidOperationException($"Organization {Id} doesn't exist.");
        }

        var getDetailsTasks = _geofenceGrains
            .Select(geofence => geofence.GetDetails())
            .ToArray();

        return await Task.WhenAll(getDetailsTasks);
    }
}