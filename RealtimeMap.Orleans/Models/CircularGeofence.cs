using GeoCoordinatePortable;

namespace RealtimeMap.Orleans.Models;

public class CircularGeofence
{
    public string Name { get; }
    public double RadiusInMetres { get; }
    public GeoPoint CentralPoint { get; }

    private readonly GeoCoordinate _geoCoordinate;

    public CircularGeofence(string name, GeoPoint centralPoint, double radiusInMetres)
    {
        RadiusInMetres = radiusInMetres;
        Name = name;
        CentralPoint = centralPoint;
        
        _geoCoordinate = new GeoCoordinate(centralPoint.Latitude, centralPoint.Longitude);
    }

    public bool IncludesLocation(double latitude, double longitude)
    {
        return _geoCoordinate.GetDistanceTo(new GeoCoordinate(latitude, longitude)) <= RadiusInMetres;
    }
}