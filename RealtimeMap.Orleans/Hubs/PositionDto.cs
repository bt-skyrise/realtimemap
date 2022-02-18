using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.Hubs;

public class PositionDto
{
    public string VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long Timestamp { get; set; }
    public int Heading { get; set; }
    public double Speed { get; set; }
    public bool DoorsOpen { get; set; }

    public static PositionDto MapFrom(VehiclePosition position)
    {
        return new()
        {
            Latitude = position.Position.Latitude,
            Longitude = position.Position.Longitude,
            Timestamp = 0,
            Heading = position.Heading,
            VehicleId = position.VehicleId,
            Speed = position.Speed,
            DoorsOpen = false
        };
    }
}

public class PositionsDto
{
    public PositionDto[] Positions { get; set; }
}