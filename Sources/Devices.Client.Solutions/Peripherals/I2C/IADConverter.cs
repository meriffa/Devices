namespace Devices.Client.Solutions.Peripherals.I2C;

/// <summary>
/// A/D Converter interface
/// </summary>
public interface IADConverter : IDisposable
{

    #region Public Methods
    /// <summary>
    /// Read input channel
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    int ReadInput(int channel);
    #endregion

}