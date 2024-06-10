namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Base hub interface
/// </summary>
public interface IHubBase
{

    #region Public Methods
    /// <summary>
    /// Device presence confirmation request
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task DevicePresenceConfirmationRequest(string sender);

    /// <summary>
    /// Device presence confirmation response
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public Task DevicePresenceConfirmationResponse(object state);

    /// <summary>
    /// Shutdown request
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task ShutdownRequest(string sender);

    /// <summary>
    /// Shutdown response
    /// </summary>
    /// <returns></returns>
    public Task ShutdownResponse();
    #endregion

}