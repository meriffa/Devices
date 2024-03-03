using System.Device.I2c;

namespace Devices.Client.Solutions.Peripherals.I2C;

/// <summary>
/// A/D Converter (PCF8591)
/// </summary>
public sealed class PCF8591 : IADConverter
{

    #region Private Fields
    private I2cDevice? device;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public PCF8591()
    {
        try
        {
            device = I2cDevice.Create(new I2cConnectionSettings(1, 0x48));
            device.ReadByte();
        }
        catch
        {
            throw new("PCF8591 device not found.");
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
        if (channel < 0 || channel > 3)
            throw new($"Invalid channel {channel} specified.");
        var command = (byte)(0x40 + channel);
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