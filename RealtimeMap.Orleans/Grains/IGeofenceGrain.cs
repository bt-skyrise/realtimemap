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

[GenerateSerializer]
public record GeofenceDetails
{
    public GeofenceDetails()
    {
    }
    
    public GeofenceDetails(string Name,
        string OrgId,
        double RadiusInMeters,
        GeoPoint Position,
        string[] VehiclesInZone)
    {
        this.Name = Name;
        this.OrgId = OrgId;
        this.RadiusInMeters = RadiusInMeters;
        this.Position = Position;
        this.VehiclesInZone = VehiclesInZone;
    }

    [Id(0)]
    public string Name { get; init; }
    
    [Id(1)]
    public string OrgId { get; init; }
    
    [Id(2)]
    public double RadiusInMeters { get; init; }
    
    [Id(3)]
    public GeoPoint Position { get; init; }
    
    [Id(4)]
    public string[] VehiclesInZone { get; init; }
}

public static class GeofenceGrainExtensions
{
    public static IGeofenceGrain GetGeofenceGrain(this IGrainFactory grainFactory, string organizationId, string geofenceName)
    {
        var geofenceIdentity = new GeofenceIdentity(organizationId, geofenceName);
        return grainFactory.GetGrain<IGeofenceGrain>(geofenceIdentity.ToString());
    }
}