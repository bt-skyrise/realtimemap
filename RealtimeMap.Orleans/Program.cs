using RealtimeMap.Orleans;
using RealtimeMap.Orleans.Ingress;

var builder = WebApplication.CreateBuilder(args);

builder.UseRealtimeMapOrleans();
builder.UseRealtimeMapLogging();
builder.UseRealtimeMapIngres();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();