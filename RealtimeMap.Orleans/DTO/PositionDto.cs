using RealtimeMap.Orleans.Positions;

namespace RealtimeMap.Orleans.DTO;

public class PositionDto
{
    public string VehicleId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public long Timestamp { get; set; }
    public int Heading { get; set; }
    public float? Speed { get; set; }
    public bool DoorsOpen { get; set; }

    public static PositionDto MapFrom(VehiclePosition vehiclePosition)
    {
        return new()
        {
            Latitude = vehiclePosition.Position.Latitude,
            Longitude = vehiclePosition.Position.Longitude,
            Timestamp = ((DateTimeOffset)vehiclePosition.Timestamp).ToUnixTimeSeconds(),
            Heading = vehiclePosition.Heading,
            VehicleId = vehiclePosition.VehicleId,
            Speed = 10,
            DoorsOpen = vehiclePosition.DoorsOpen
        };
    }
}

public class PositionsDto
{
    public PositionDto[] Positions { get; set; }
}