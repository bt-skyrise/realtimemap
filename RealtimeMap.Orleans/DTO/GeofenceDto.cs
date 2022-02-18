using RealtimeMap.Orleans.Grains;

namespace RealtimeMap.Orleans.DTO;

public class GeofenceDto
{
    public string Name { get; set; }
    public double Longitude { get; set; }
    public double Latitude { get; set; }
    public double RadiusInMeters { get; set; }
    public string[] VehiclesInZone { get; set; }

    public static GeofenceDto FromGeofenceDetails(GeofenceDetails geofence)
    {
        return new GeofenceDto
        {
            Name = geofence.Name,
            RadiusInMeters = geofence.RadiusInMeters,
            Longitude = geofence.Position.Longitude,
            Latitude = geofence.Position.Latitude,
            VehiclesInZone = Array.Empty<string>()
        };
    }
}