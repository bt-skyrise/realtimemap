namespace RealtimeMap.Orleans.Models;

public record Notification(
    string VehicleId,
    string OrganizationId,
    string OrganizationName,
    string GeofenceName,
    GeofenceEvent GeofenceEvent
);