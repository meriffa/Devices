using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Interfaces.Identification;
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
public class GardenHub(ILogger<GardenHub> logger, IOptions<ClientOptions> options, IIdentityService identityService) : IGardenHub
{

    #region Private Fields
    private readonly ILogger<GardenHub> logger = logger;
    private readonly HubConnection connection = GetHubConnection(logger, options.Value, identityService);
    private string sender = string.Empty;
    #endregion

    #region Public Methods
    /// <summary>
    /// Start hub client
    /// </summary>
    public void Start()
    {
        Task.Run(async () =>
        {
            await connection.StartAsync();
            logger.LogInformation("Hub connection established (Connection ID = '{connection.ConnectionId}').", connection.ConnectionId);
        }).Wait();
    }

    /// <summary>
    /// Stop hub client
    /// </summary>
    public void Stop()
    {
        Task.Run(async () =>
        {
            try
            {
                await connection.StopAsync();
                logger.LogInformation("Hub connection closed (Connection ID = '{connection.ConnectionId}').", connection.ConnectionId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        }).Wait();
    }

    /// <summary>
    /// Handle device presence confirmation request
    /// </summary>
    public void HandleDevicePresenceConfirmationRequest()
    {
        connection.On<string>("DevicePresenceConfirmationRequest", async (sender) =>
        {
            try
            {
                logger.LogInformation("Device presence confirmation request received (Sender = {sender}).", this.sender = sender);
                await connection.InvokeAsync("SendDevicePresenceConfirmationResponse", sender);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Handle pump request
    /// </summary>
    /// <param name="action"></param>
    public void HandlePumpRequest(Action<int, bool> action)
    {
        connection.On<string, int, bool>("PumpRequest", async (sender, pumpIndex, pumpState) =>
        {
            try
            {
                logger.LogInformation("Pump request received (Sender = {sender}, Pump Index = {pumpIndex}, Pump State = {pumpState}).", this.sender = sender, pumpIndex, pumpState);
                action(pumpIndex, pumpState);
                await connection.InvokeAsync("SendPumpResponse", sender, pumpIndex, pumpState, null);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
                await connection.InvokeAsync("SendPumpResponse", sender, pumpIndex, pumpState, ex.Message);
            }
        });
    }

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    public void SendPresenceConfirmationRequest()
    {
        Task.Run(async () =>
        {
            try
            {
                await connection.InvokeAsync("SendPresenceConfirmationRequest", sender);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        }).Wait();
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
    public void HandleShutdownRequest(Action action)
    {
        connection.On<string>("ShutdownRequest", (sender) =>
        {
            try
            {
                logger.LogInformation("Shutdown request received (Sender = {sender}).", this.sender = sender);
                action();
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
    public void SendShutdownResponse()
    {
        Task.Run(async () =>
        {
            try
            {
                await connection.InvokeAsync("SendShutdownResponse", sender);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        }).Wait();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return hub connection
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="clientOptions"></param>
    /// <returns></returns>
    private static HubConnection GetHubConnection(ILogger<GardenHub> logger, ClientOptions clientOptions, IIdentityService identityService)
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
                connectionOptions.AccessTokenProvider = () => Task.FromResult((string?)identityService.GetDeviceBearerToken());
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