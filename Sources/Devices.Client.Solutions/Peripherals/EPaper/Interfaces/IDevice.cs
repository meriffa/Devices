namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Device interface
/// </summary>
public interface IDevice : IDisposable
{

    #region Properties
    /// <summary>
    /// Display width
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Display height
    /// </summary>
    int Height { get; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <returns></returns>
    bool WaitUntilReady();

    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    bool WaitUntilReady(int timeout);

    /// <summary>
    /// Power controller on (do not use with sleep mode)
    /// </summary>
    void PowerOn();

    /// <summary>
    /// Power controller off (do not use with sleep mode)
    /// </summary>
    void PowerOff();

    /// <summary>
    /// Enter sleep mode
    /// </summary>
    void Sleep();

    /// <summary>
    /// Wake up from sleep mode
    /// </summary>
    void WakeUp();

    /// <summary>
    /// Clear display to white
    /// </summary>
    void Clear();

    /// <summary>
    /// Clear display to black
    /// </summary>
    void ClearBlack();

    /// <summary>
    /// Reset display
    /// </summary>
    void Reset();
    #endregion

}