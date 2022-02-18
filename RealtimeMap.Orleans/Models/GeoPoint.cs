using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Models;

public record GeoPoint(double Longitude, double Latitude)
{
    public bool IsWithinViewport(Viewport viewport)
    {
        // naive implementation, ignores edge cases
        return Longitude >= viewport.SouthWest.Longitude &&
               Latitude >= viewport.SouthWest.Longitude &&
               Longitude <= viewport.NorthEast.Longitude &&
               Latitude <= viewport.NorthEast.Latitude;
    }
}