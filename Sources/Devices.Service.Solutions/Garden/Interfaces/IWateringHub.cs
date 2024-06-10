namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Watering hub interface
/// </summary>
public interface IWateringHub : IHubBase
{

    #region Public Methods
    /// <summary>
    /// Pump request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    public Task PumpRequest(string sender, int pumpIndex, bool pumpState);

    /// <summary>
    /// Pump response
    /// </summary>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public Task PumpResponse(int pumpIndex, bool pumpState, string? error);

    /// <summary>
    /// Operator presence confirmation request
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task OperatorPresenceConfirmationRequest(string sender);

    /// <summary>
    /// Operator presence confirmation response
    /// </summary>
    /// <returns></returns>
    public Task OperatorPresenceConfirmationResponse();
    #endregion

}