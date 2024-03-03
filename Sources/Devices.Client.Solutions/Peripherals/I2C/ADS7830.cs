using System.Device.I2c;

namespace Devices.Client.Solutions.Peripherals.I2C;

/// <summary>
/// A/D Converter (ADS7830)
/// </summary>
public sealed class ADS7830 : IADConverter
{

    #region Private Fields
    private I2cDevice? device;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public ADS7830()
    {
        try
        {
            device = I2cDevice.Create(new I2cConnectionSettings(1, 0x4B));
            device.ReadByte();
        }
        catch
        {
            throw new("ADS7830 device not found.");
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Read input channel
    /// </summary>
    /// <param name="channel"></param>
    /// <returns></returns>
    public int ReadInput(int channel)
    {
        if (channel < 0 || channel > 8)
            throw new($"Invalid channel {channel} specified.");
        var command = (byte)(0x84 | (((channel << 2 | channel >> 1) & 0x07) << 4) );
        device!.WriteByte(command);
        return device.ReadByte();
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        device?.Dispose();
        device = null;
    }
    #endregion

}