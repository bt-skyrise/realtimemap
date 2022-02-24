using Orleans;
using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Models;

[GenerateSerializer]
public class GeoPoint
{
    [Id(0)]
    public double Longitude { get; init; }
    
    [Id(1)]
    public double Latitude { get; init; }

    public GeoPoint()
    {
    }

    public GeoPoint(double longitude, double latitude)
    {
        Longitude = longitude;
        Latitude = latitude;
    }
    
    public bool IsWithinViewport(Viewport viewport)
    {
        // naive implementation, ignores edge cases
        return Longitude >= viewport.SouthWest.Longitude &&
               Latitude >= viewport.SouthWest.Longitude &&
               Longitude <= viewport.NorthEast.Longitude &&
               Latitude <= viewport.NorthEast.Latitude;
    }
}