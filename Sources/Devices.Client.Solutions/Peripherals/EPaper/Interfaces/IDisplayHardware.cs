using System.Device.Gpio;

namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Display hardware interface
/// </summary>
public interface IDisplayHardware : IDisposable
{

    #region Properties
    /// <summary>
    /// GPIO reset pin
    /// </summary>
    PinValue ResetPin { get; set; }

    /// <summary>
    /// GPIO SPI DC pin
    /// </summary>
    PinValue SpiDcPin { get; set; }

    /// <summary>
    /// GPIO busy pin
    /// </summary>
    PinValue BusyPin { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Write data to SPI device
    /// </summary>
    /// <param name="stream"></param>
    void Write(MemoryStream stream);

    /// <summary>
    /// Write data to SPI device
    /// </summary>
    /// <param name="buffer"></param>
    void Write(byte[] buffer);

    /// <summary>
    /// Write a byte to SPI device
    /// </summary>
    /// <param name="value"></param>
    void WriteByte(byte value);
    #endregion

}