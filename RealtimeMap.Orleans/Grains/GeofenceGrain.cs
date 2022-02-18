using Orleans;
using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Grains;

public class GeofenceGrain : RealtimeMapGrain, IGeofenceGrain
{
    private CircularGeofence? _geofence;
    
    private GeofenceIdentity Id => GeofenceIdentity.FromString(this.GetPrimaryKeyString());

    public override Task OnActivateAsync()
    {
        _geofence = Organizations
            .ById[Id.OrganizationId]
            .Geofences
            .SingleOrDefault(geofence => geofence.Name == Id.GeofenceName);

        return Task.CompletedTask;
    }

    public Task<GeofenceDetails> GetDetails()
    {
        if (_geofence is null)
        {
            throw new InvalidOperationException($"Geofence {Id} was not initialized.");
        }
        
        return Task.FromResult(new GeofenceDetails(
            Name: Id.GeofenceName,
            OrgId: Id.OrganizationId,
            RadiusInMeters: _geofence.RadiusInMetres,
            Position: _geofence.CentralPoint
        ));
    }
}