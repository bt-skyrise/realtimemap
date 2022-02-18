using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.DTO;

public class NotificationDto
{
    public string VehicleId { get; set; }
    
    public string OrgId { get; set; }
    
    public string OrgName { get; set; }

    public string ZoneName { get; set; }
    
    public string Event { get; set; }

    public static NotificationDto MapFrom(Notification notification)
    {
        return new()
        {
            VehicleId = notification.VehicleId,
            OrgId = notification.OrganizationId,
            OrgName = notification.OrganizationName,
            ZoneName = notification.GeofenceName,
            Event = notification.GeofenceEvent.ToString()
        };
    }
}