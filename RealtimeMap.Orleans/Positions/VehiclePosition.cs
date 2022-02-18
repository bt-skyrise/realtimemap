namespace RealtimeMap.Orleans.Positions;

public record VehiclePosition(
    string OrgId,
    GeoPoint Position,
    string VehicleId,
    DateTime Timestamp,
    int Heading,
    bool DoorsOpen,
    double Speed
)
{
    public bool IsWithinViewport(Viewport viewport) => Position.IsWithinViewport(viewport);
}