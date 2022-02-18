namespace RealtimeMap.Orleans.Positions;

public record GeoPoint(double Longitude, double Latitude)
{
    public bool IsInViewport(Viewport viewport)
    {
        // naive implementation, ignores edge cases
        return Longitude >= viewport.SouthWest.Longitude &&
               Latitude >= viewport.SouthWest.Longitude &&
               Longitude <= viewport.NorthEast.Longitude &&
               Latitude <= viewport.NorthEast.Latitude;
    }
}