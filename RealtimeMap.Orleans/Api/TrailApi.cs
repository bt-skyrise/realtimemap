using Orleans;
using RealtimeMap.Orleans.DTO;
using RealtimeMap.Orleans.Grains;

namespace RealtimeMap.Orleans.Api;

public static class TrailApi
{
    public static void MapTrailApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/trail/{vehicleId}", async (string vehicleId, IClusterClient clusterClient) =>
        {
            var positionsHistory = await clusterClient
                .GetGrain<IVehicleGrain>(vehicleId)
                .GetPositionsHistory();
            
            return Results.Ok(new PositionsDto
            {
                Positions = positionsHistory
                    .Select(PositionDto.MapFrom)
                    .ToArray()
            });
        });
    }
}