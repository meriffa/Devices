using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devices.Client.Solutions.Garden.Hubs;

/// <summary>
/// Base hub
/// </summary>
public abstract class HubBase : IHubBase
{

    #region Private Fields
    private readonly string url;
    private readonly ILogger<HubBase> logger;
    private Action? shutdownAction;
    #endregion

    #region Protected Fields
    protected readonly HubConnection connection;
    protected string sender = string.Empty;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="url"></param>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    /// <param name="identityService"></param>
    public HubBase(string url, ILogger<HubBase> logger, IOptions<ClientOptions> options, IIdentityService identityService)
    {
        this.url = url;
        this.logger = logger;
        connection = GetHubConnection(options.Value, identityService);
    }
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
            logger.LogInformation("Hub connection established (URL = '{url}', Connection ID = '{connection.ConnectionId}').", url, connection.ConnectionId);
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
                logger.LogInformation("Hub connection closed (URL = '{url}').", url);
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
    /// <param name="getDevicePresenceConfirmation"></param>
    public void HandleDevicePresenceConfirmationRequest(Func<object?> getDevicePresenceConfirmation)
    {
        connection.On<string>("DevicePresenceConfirmationRequest", async (sender) =>
        {
            try
            {
                logger.LogInformation("Device presence confirmation request received (Sender = {sender}).", this.sender = sender);
                await connection.InvokeAsync("SendDevicePresenceConfirmationResponse", sender, getDevicePresenceConfirmation());
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
    /// <param name="shutdownAction"></param>
    public void HandleShutdownRequest(Action shutdownAction)
    {
        this.shutdownAction = shutdownAction;
        connection.On<string>("ShutdownRequest", (sender) =>
        {
            try
            {
                logger.LogInformation("Shutdown request received (Sender = {sender}).", this.sender = sender);
                shutdownAction();
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
    /// <param name="clientOptions"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    private HubConnection GetHubConnection(ClientOptions clientOptions, IIdentityService identityService)
    {
        var connection = new HubConnectionBuilder()
            .WithUrl($"{clientOptions.Service.Host}{url}", (connectionOptions) =>
            {
                connectionOptions.HttpMessageHandlerFactory = (message) =>
                {
                    if (message is HttpClientHandler clientHandler)
                        clientHandler.ServerCertificateCustomValidationCallback += (sender, certificate, chain, sslPolicyErrors) => { return true; };
                    return message;
                };
                connectionOptions.AccessTokenProvider = () => Task.FromResult((string?)identityService.GetDeviceBearerToken());
            })
            .WithAutomaticReconnect([TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(35)])
            .Build();
        connection.Reconnecting += error =>
        {
            logger.LogInformation("Hub connection lost (URL = '{url}', Error = {error}).", url, error?.Message ?? "N/A");
            return Task.CompletedTask;
        };
        connection.Reconnected += connectionId =>
        {
            logger.LogInformation("Hub connection reestablished (URL = '{url}', Connection ID = '{connectionId}').", url, connectionId);
            return Task.CompletedTask;
        };
        connection.Closed += (error) =>
        {
            if (error != null)
            {
                logger.LogInformation("Hub connection terminated (URL = '{url}', Error = {error}).", url, error.Message);
                shutdownAction?.Invoke();
            }
            return Task.CompletedTask;
        };
        return connection;
    }
    #endregion

}