using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devices.Client.Solutions.Garden.Hubs;

/// <summary>
/// Garden hub
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class GardenHub(ILogger<GardenHub> logger, IOptions<ClientOptions> options) : IGardenHub
{

    #region Private Fields
    private readonly ILogger<GardenHub> logger = logger;
    private readonly HubConnection connection = GetHubConnection(logger, options.Value);
    #endregion

    #region Public Methods
    /// <summary>
    /// Start hub client
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Start()
    {
        await connection.StartAsync();
        logger.LogInformation("Hub connection established (Connection ID = '{connection.ConnectionId}').", connection.ConnectionId);
        return true;
    }

    /// <summary>
    /// Stop hub client
    /// </summary>
    /// <returns></returns>
    public async Task<bool> Stop()
    {
        await connection.StopAsync();
        logger.LogInformation("Hub connection closed (Connection ID = '{connection.ConnectionId}').", connection.ConnectionId);
        return true;
    }

    /// <summary>
    /// Handle pump request
    /// </summary>
    /// <param name="action"></param>
    public void HandlePumpRequest(Action<int, int, bool> action)
    {
        connection.On<int, int, bool>("PumpRequest", (deviceId, pumpIndex, pumpState) =>
        {
            try
            {
                logger.LogInformation("PumpRequest: Device ID = {deviceId}, Pump Index = {pumpIndex}, Pump State = {pumpState}.", deviceId, pumpIndex, pumpState);
                action(deviceId, pumpIndex, pumpState);
                connection.InvokeAsync("SendPumpResponse", deviceId, pumpIndex, pumpState, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
                connection.InvokeAsync("SendPumpResponse", deviceId, pumpIndex, pumpState, ex.Message);
            }
        });
    }

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    public void SendPresenceConfirmationRequest()
    {
        try
        {
            Task.Run(async () => await connection.InvokeAsync("SendPresenceConfirmationRequest"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
        }
    }

    /// <summary>
    /// Handle presence confirmation response
    /// </summary>
    /// <param name="action"></param>
    public void HandlePresenceConfirmationResponse(Action action)
    {
        connection.On("PresenceConfirmationResponse", () =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Handle shutdown request
    /// </summary>
    /// <param name="action"></param>
    public void HandleShutdownRequest(Action<int> action)
    {
        connection.On<int>("ShutdownRequest", (deviceId) =>
        {
            try
            {
                logger.LogInformation("ShutdownRequest: Device ID = {deviceId}.", deviceId);
                action(deviceId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Send shutdown response
    /// </summary>
    /// <param name="deviceId"></param>
    public void SendShutdownResponse(int deviceId)
    {
        try
        {
            Task.Run(async () => await connection.InvokeAsync("SendShutdownResponse", deviceId));
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return hub connection
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="clientOptions"></param>
    /// <returns></returns>
    private static HubConnection GetHubConnection(ILogger<GardenHub> logger, ClientOptions clientOptions)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{clientOptions.Service.Host}/Hub/Solutions/Garden", (connectionOptions) =>
            {
                connectionOptions.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        clientHandler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    return message;
                };
            }).WithAutomaticReconnect().Build();
        connection.Reconnected += connectionId =>
        {
            logger.LogInformation("Hub connection reestablished (Connection ID = '{connectionId}').", connectionId);
            return Task.CompletedTask;
        };
        connection.Closed += async (error) =>
        {
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();
        };
        return connection;
    }
    #endregion

}