using Orleans;
using RealtimeMap.Orleans.DTO;
using RealtimeMap.Orleans.Grains;
using RealtimeMap.Orleans.Models;

namespace RealtimeMap.Orleans.Api;

public static class OrganizationApi
{
    public static void MapOrganizationApi(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/organization", () =>
        {
            var results = Organizations.All
                .Where(organization => organization.Geofences.Any())
                .Select(organization => new OrganizationDto
                {
                    Id = organization.Id,
                    Name = organization.Name
                })
                .OrderBy(organization => organization.Name)
                .ToList();

            return Results.Ok(results);
        });

        app.MapGet("/api/organization/{id}", async (string id, IClusterClient clusterClient) =>
        {
            if (!Organizations.ById.TryGetValue(id, out var organization))
            {
                return Results.NotFound();
            }

            var organizationGrain = clusterClient.GetGrain<IOrganizationGrain>(id);

            var geofences = await organizationGrain.GetGeofences();

            return Results.Ok(new OrganizationDetailsDto
            {
                Id = id,
                Name = organization.Name,
                
                Geofences = geofences
                    .Select(GeofenceDto.FromGeofenceDetails)
                    .OrderBy(geofence => geofence.Name)
                    .ToArray()
            });
        });
    }
}