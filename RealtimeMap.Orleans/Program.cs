using RealtimeMap.Orleans;

var builder = WebApplication.CreateBuilder(args);

builder.UseRealtimeMapOrleans();
builder.UseRealtimeMapLogging();
builder.UseRealtimeMapIngres();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();