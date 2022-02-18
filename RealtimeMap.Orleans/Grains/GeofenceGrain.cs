using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public class GeofenceGrain : RealtimeMapGrain, IGeofenceGrain
{
    private IAsyncStream<Notification>? _notificationsStream;
    private Organization? _organization;
    private CircularGeofence? _geofence;
    private readonly HashSet<string> _vehiclesInGeofence = new();

    private GeofenceIdentity Id => GeofenceIdentity.FromString(this.GetPrimaryKeyString());
    
    public override async Task OnActivateAsync()
    {
        _notificationsStream = GetNotificationsStream();

        _organization = Organizations
            .ById[Id.OrganizationId];
        
        _geofence = _organization
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
                await SendNotification(vehiclePosition, GeofenceEvent.Enter);
            }
        }
        else
        {
            if (vehicleAlreadyInZone)
            {
                _vehiclesInGeofence.Remove(vehiclePosition.VehicleId);
                await SendNotification(vehiclePosition, GeofenceEvent.Exit);
            }
        }
    }

    private async Task SendNotification(VehiclePosition vehiclePosition, GeofenceEvent geofenceEvent)
    {
        await _notificationsStream!.OnNextAsync(new Notification(
            VehicleId: vehiclePosition.VehicleId,
            OrganizationId: _organization!.Id,
            OrganizationName: _organization!.Name,
            GeofenceName: _geofence!.Name,
            GeofenceEvent: geofenceEvent
        ));
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