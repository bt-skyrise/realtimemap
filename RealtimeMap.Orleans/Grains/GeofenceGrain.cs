using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public class GeofenceGrain : RealtimeMapGrain, IGeofenceGrain
{
    private IAsyncStream<VehiclePosition>? _positionsStream;
    private CircularGeofence? _geofence;
    private readonly HashSet<string> _vehiclesInGeofence = new();
    
    private GeofenceIdentity Id => GeofenceIdentity.FromString(this.GetPrimaryKeyString());
    
    public override async Task OnActivateAsync()
    {
        _geofence = Organizations
            .ById[Id.OrganizationId]
            .Geofences
            .SingleOrDefault(geofence => geofence.Name == Id.GeofenceName);
    }

    public async Task OnPosition(VehiclePosition vehiclePosition)
    {
        if (_geofence is null)
        {
            return;
        }
        
        // todo: use logging
        Console.WriteLine($"Geofence {Id}: received {vehiclePosition}");
        
        var vehicleAlreadyInZone = _vehiclesInGeofence.Contains(vehiclePosition.VehicleId);
        
        if (_geofence.IncludesLocation(vehiclePosition.Position))
        {
            if (!vehicleAlreadyInZone)
            {
                _vehiclesInGeofence.Add(vehiclePosition.VehicleId);
                
                // todo: send notification
            }
        }
        else
        {
            if (vehicleAlreadyInZone)
            {
                _vehiclesInGeofence.Remove(vehiclePosition.VehicleId);
                
                // todo: send notification
            }
        }
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
            Position: _geofence.CentralPoint,
            VehiclesInZone: _vehiclesInGeofence.ToArray()
        ));
    }
}