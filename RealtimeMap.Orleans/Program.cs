using RealtimeMap.Orleans;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureLogging();

builder.Services.AddHostedService<IngressHostedService>();

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();