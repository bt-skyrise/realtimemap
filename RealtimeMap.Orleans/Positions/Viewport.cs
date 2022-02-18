using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Positions;

public record Viewport(GeoPoint SouthWest, GeoPoint NorthEast);