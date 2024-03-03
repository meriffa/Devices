namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Display writer interface
/// </summary>
public interface IDisplayWriter : IDisposable
{

    #region Public Methods
    /// <summary>
    /// Write buffer data
    /// </summary>
    /// <param name="index"></param>
    void Write(int index);

    /// <summary>
    /// Write blank line
    /// </summary>
    void WriteBlankLine();

    /// <summary>
    /// Write blank pixel
    /// </summary>
    void WriteBlankPixel();

    /// <summary>
    /// Send buffer data to display
    /// </summary>
    void Finish();
    #endregion

}