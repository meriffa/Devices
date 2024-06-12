using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Options;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Devices.Client.Solutions.Garden.Hubs;

/// <summary>
/// Camera hub
/// </summary>
public class CameraHub(ILogger<CameraHub> logger, IOptions<ClientOptions> options, IIdentityService identityService) : HubBase("/Hub/Solutions/Camera", logger, options, identityService), ICameraHub
{

    #region Public Methods
    /// <summary>
    /// Handle pan request
    /// </summary>
    /// <param name="action"></param>
    public void HandlePanRequest(Action<int> action)
    {
        connection.On<string, int>("PanRequest", (sender, value) =>
        {
            try
            {
                logger.LogInformation("Camera pan request received (Sender = {sender}, Value = {value}).", this.sender = sender, value);
                action(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Handle tilt request
    /// </summary>
    /// <param name="action"></param>
    public void HandleTiltRequest(Action<int> action)
    {
        connection.On<string, int>("TiltRequest", (sender, value) =>
        {
            try
            {
                logger.LogInformation("Camera tilt request received (Sender = {sender}, Value = {value}).", this.sender = sender, value);
                action(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Handle focus request
    /// </summary>
    /// <param name="action"></param>
    public void HandleFocusRequest(Action<double> action)
    {
        connection.On<string, double>("FocusRequest", (sender, value) =>
        {
            try
            {
                logger.LogInformation("Camera focus request received (Sender = {sender}, Value = {value}).", this.sender = sender, value);
                action(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }

    /// <summary>
    /// Handle zoom request
    /// </summary>
    /// <param name="action"></param>
    public void HandleZoomRequest(Action<double> action)
    {
        connection.On<string, double>("ZoomRequest", (sender, value) =>
        {
            try
            {
                logger.LogInformation("Camera zoom request received (Sender = {sender}, Value = {value}).", this.sender = sender, value);
                action(value);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Error}", ex.Message);
            }
        });
    }
    #endregion

}