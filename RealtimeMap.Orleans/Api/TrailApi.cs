using Orleans;
using RealtimeMap.Orleans.DTO;

namespace RealtimeMap.Orleans.Api;

public static class TrailApi
{
    public static void MapTrailApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/trail/{vehicleId}", async (string vehicleId, IClusterClient cluster) =>
        {
            throw new NotImplementedException();

            // var positionsHistory = await cluster
            //     .GetVehicleActor(vehicleId)
            //     .GetPositionsHistory(
            //         new GetPositionsHistoryRequest(), 
            //         cluster.System.Root.WithTracing(),
            //         CancellationToken.None);
            //
            // var positions = positionsHistory.Positions
            //     .Select(PositionDto.MapFrom)
            //     .ToArray();
            //
            // var result = new PositionsDto
            // {
            //     Positions = positions
            // };
            //
            // return Results.Ok(result);
        });
    }
}