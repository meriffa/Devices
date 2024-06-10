using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devices.Client.Solutions.Garden.Hubs;

/// <summary>
/// Watering hub
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class WateringHub(ILogger<WateringHub> logger, IOptions<ClientOptions> options, IIdentityService identityService) : HubBase("/Hub/Solutions/Watering", logger, options, identityService), IWateringHub
{

    #region Public Methods
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
    /// Send operator presence confirmation request
    /// </summary>
    public void SendOperatorPresenceConfirmationRequest()
    {
        Task.Run(async () =>
        {
            try
            {
                await connection.InvokeAsync("SendOperatorPresenceConfirmationRequest", sender);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        }).Wait();
    }

    /// <summary>
    /// Handle operator presence confirmation response
    /// </summary>
    /// <param name="action"></param>
    public void HandleOperatorPresenceConfirmationResponse(Action action)
    {
        connection.On("OperatorPresenceConfirmationResponse", () =>
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
    #endregion

}