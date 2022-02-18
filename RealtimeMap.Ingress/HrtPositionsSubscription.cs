﻿using System.Security.Authentication;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Protocol;

namespace RealtimeMap.Ingress;

// todo: include metrics again

public class HrtPositionsSubscription : IDisposable
{
    public static async Task<HrtPositionsSubscription> Start(
        string sharedSubscriptionGroupName,
        Func<HrtPositionUpdate, Task> onPositionUpdate,
        ILoggerFactory loggerFactory,
        CancellationToken cancel)
    {
        var logger = loggerFactory.CreateLogger<HrtPositionsSubscription>();

        var mqttClient = CreateMqttClient();

        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithClientId(null) // do not keep state on the broker
            .WithCleanSession()
            .WithNoKeepAlive()
            .WithTls(new MqttClientOptionsBuilderTlsParameters
            {
                UseTls = true,
                SslProtocol = SslProtocols.Tls12
            })
            .WithTcpServer("mqtt.hsl.fi", 8883)
            .Build();

        mqttClient.UseConnectedHandler(async args =>
        {
            logger.LogInformation("Connected to MQTT Server");

            // we subscribe to a group subscription, so messages are distributed between cluster nodes
            await mqttClient.SubscribeAsync(
                $"$share/{sharedSubscriptionGroupName}//hfp/v2/journey/ongoing/vp/bus/#",
                MqttQualityOfServiceLevel.AtMostOnce);

            logger.LogInformation("Subscribed to MQTT topic");
        });

        mqttClient.UseDisconnectedHandler(async args =>
        {
            if (args.Exception != null)
                logger.LogWarning(args.Exception, "Disconnected from MQTT server");
            else
                logger.LogWarning("Disconnected from MQTT server");

            try
            {
                logger.LogInformation("Attempting reconnect to MQTT server");
                await mqttClient.ConnectAsync(mqttClientOptions, cancel);
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Unable to reconnect to to MQTT server");
            }
        });
        
        mqttClient.UseApplicationMessageReceivedHandler(args =>
        {
            async Task Inner()
            {
                try
                {
                    logger.LogDebug("Received message on {@Topic}", args.ApplicationMessage.Topic);

                    var hrtPositionUpdate = HrtPositionUpdate.ParseFromMqttMessage(args.ApplicationMessage);

                    if (hrtPositionUpdate.HasValidPosition)
                        await onPositionUpdate(hrtPositionUpdate);
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Error while processing message {@Message}", args.ApplicationMessage);
                }
            }

            return Inner();
        });

        logger.LogInformation("Connecting to MQTT server");
        await mqttClient.ConnectAsync(mqttClientOptions, cancel);

        return new HrtPositionsSubscription(mqttClient);
    }

    private static IMqttClient CreateMqttClient()
    {
        var factory = new MqttFactory(new SerilogMqttNetLogger());

        return factory.CreateMqttClient();
    }

    private readonly IMqttClient _mqttClient;

    private HrtPositionsSubscription(IMqttClient mqttClient) => _mqttClient = mqttClient;

    public void Dispose() => _mqttClient?.Dispose();
}