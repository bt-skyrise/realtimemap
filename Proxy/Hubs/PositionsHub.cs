using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Backend;
using Grpc.Core;
using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;

namespace Proxy.Hubs
{
    class PositionHubState
    {
        public MapBackend.MapBackendClient Client;
        public AsyncDuplexStreamingCall<CommandEnvelope, PositionBatch> GrpcConnection;
    }

    [PublicAPI]
    public class PositionsHub : Hub
    {
        public string ConnectionId => $"connection{Context.ConnectionId}";

        private PositionHubState State
        {
            get
            {
                if (!Context.Items.ContainsKey("state")) Context.Items.Add("state", new PositionHubState());

                return Context.Items["state"] as PositionHubState;
            }
        }

        public async IAsyncEnumerable<PositionsDto> Connect()
        {
            Console.WriteLine("Connect user session " + ConnectionId);

            var channel =
                new Channel("127.0.0.1", 5002,
                    ChannelCredentials.Insecure); //GrpcChannel.ForAddress(new Uri("https://localhost:4040"));

            Console.WriteLine("Got response");

            var grpcClient = new MapBackend.MapBackendClient(channel);
            var conn = grpcClient.Connect();
            
            State.Client = grpcClient;
            State.GrpcConnection = conn;

            Console.WriteLine("Got response...");

            CommandEnvelope envelope = new()
            {
                CreateViewport = new CreateViewport()
                {
                    ConnectionId = ConnectionId
                }
            };

            await conn.RequestStream.WriteAsync(envelope);

            Console.WriteLine($"Connected {envelope}");

            var responseStream = State.GrpcConnection.ResponseStream;

            await foreach (var positionBatch in responseStream.ReadAllAsync())
            {
                if (!positionBatch.Positions.Any())
                    continue;

                var dtoBatch = positionBatch.Positions.Select(MapPositionDto).ToArray();
                yield return new PositionsDto
                {
                    Positions = dtoBatch,
                };
            }
        }

        public async Task SetViewport(double lng1, double lat1, double lng2, double lat2)
        {
            CommandEnvelope envelope = new()
            {
                UpdateViewport = new UpdateViewport
                {
                    Viewport = new Viewport
                    {
                        Lat1 = lat1,
                        Lat2 = lat2,
                        Lng1 = lng1,
                        Lng2 = lng2
                    }
                }
            };

            await State.GrpcConnection.RequestStream.WriteAsync(envelope);
        }

        public async Task<PositionsDto> GetTrail(string assetId)
        {
            var trail = await State.Client.GetTrailAsync(new GetTrailRequest { AssetId = assetId });

            var positions = trail.PositionBatch.Positions.Select(MapPositionDto).ToArray();

            return new PositionsDto
            {
                Positions = positions
            };
        }

        private static PositionDto MapPositionDto(Position position) =>
            new()
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude,
                Timestamp = position.Timestamp,
                Heading = position.Heading,
                VehicleId = position.VehicleId,
                Speed = 10,
                DoorsOpen = position.DoorsOpen
            };
    }
}