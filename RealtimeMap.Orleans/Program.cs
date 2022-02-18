using RealtimeMap.Orleans;
using RealtimeMap.Orleans.Api;
using RealtimeMap.Orleans.Hubs;
using RealtimeMap.Orleans.Ingress;

var builder = WebApplication.CreateBuilder(args);

builder.UseRealtimeMapOrleans();
builder.UseRealtimeMapLogging();
builder.UseRealtimeMapIngres();

builder.Services.AddControllers();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseCors(b => b
    .WithOrigins("http://localhost:8080")
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials()
);

app.UseRouting();

app.MapHub<EventsHub>("/events");

app.MapOrganizationApi();
app.MapTrailApi();

app.Run();