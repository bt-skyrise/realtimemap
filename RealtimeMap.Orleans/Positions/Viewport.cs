using Orleans;
using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Positions;

[GenerateSerializer]
public record Viewport
{
    public Viewport()
    {
    }
    
    public Viewport(GeoPoint SouthWest, GeoPoint NorthEast)
    {
        this.SouthWest = SouthWest;
        this.NorthEast = NorthEast;
    }

    [Id(0)]
    public GeoPoint? SouthWest { get; init; }
    
    [Id(1)]
    public GeoPoint? NorthEast { get; init; }
}