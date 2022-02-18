using Orleans;
using RealtimeMap.Orleans.Models;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Grains;

public interface IGeofenceGrain : IGrainWithStringKey
{
    Task OnPosition(VehiclePosition vehiclePosition);
    Task<GeofenceDetails> GetDetails();
}

public record GeofenceIdentity(string OrganizationId, string GeofenceName)
{
    public static GeofenceIdentity FromString(string identity)
    {
        var parts = identity.Split("/");
        return new GeofenceIdentity(parts[0], parts[1]);
    }

    public override string ToString() => $"{OrganizationId}/{GeofenceName}";
}

public record GeofenceDetails(
    string Name,
    string OrgId,
    double RadiusInMeters,
    GeoPoint Position,
    string[] VehiclesInZone
);

public static class GeofenceGrainExtensions
{
    public static IGeofenceGrain GetGeofenceGrain(this IGrainFactory grainFactory, string organizationId, string geofenceName)
    {
        var geofenceIdentity = new GeofenceIdentity(organizationId, geofenceName);
        return grainFactory.GetGrain<IGeofenceGrain>(geofenceIdentity.ToString());
    }
}