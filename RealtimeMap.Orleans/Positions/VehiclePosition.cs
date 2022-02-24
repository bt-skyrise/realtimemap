using Orleans;
using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Positions;

[GenerateSerializer]
public class VehiclePosition
{
    [Id(0)]
    public string? OrgId { get; init; }

    [Id(1)]
    public GeoPoint? Position { get; init; }

    [Id(2)]
    public string? VehicleId { get; init; }

    [Id(3)]
    public DateTime Timestamp { get; init; }

    [Id(4)]
    public int Heading { get; init; }

    [Id(5)]
    public bool DoorsOpen { get; init; }

    [Id(6)]
    public double Speed { get; init; }
    
    public bool IsWithinViewport(Viewport viewport) => Position?.IsWithinViewport(viewport) ?? false;
}