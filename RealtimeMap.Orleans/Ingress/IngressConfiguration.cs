﻿namespace RealtimeMap.Orleans.Ingress;

public static class IngressConfiguration
{
    public static void UseRealtimeMapIngres(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<IngressHostedService>();
    }
}