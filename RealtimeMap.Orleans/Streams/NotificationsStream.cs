using Orleans;
using Orleans.Streams;
using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Streams;

public static class NotificationsStream
{
    public static readonly string Namespace = "Notifications";
    public static readonly Guid Id = new("799bbd6f-821b-4d34-920b-ae7ac4415ca6");
    
    public static IAsyncStream<Notification> GetNotificationsStream(this IClusterClient client)
    {
        return client
            .GetMyStreamProvider()
            .GetNotificationsStream();
    }
    
    public static IAsyncStream<Notification> GetNotificationsStream(this IStreamProvider streamProvider)
    {
        return streamProvider.GetStream<Notification>(Id, Namespace);
    }
}