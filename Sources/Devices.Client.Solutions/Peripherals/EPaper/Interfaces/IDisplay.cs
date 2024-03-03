using Devices.Client.Solutions.Peripherals.EPaper.Common;

namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Display interface
/// </summary>
public interface IDisplay : IDevice
{

    #region Properties
    /// <summary>
    /// Color bytes per pixel (RGB)
    /// </summary>
    int ColorBytesPerPixel { get; set; }

    /// <summary>
    /// Supported device colors
    /// </summary>
    byte[] DeviceColors { get; }

    /// <summary>
    /// Device pixels per byte
    /// </summary>
    int PixelsPerByte { get; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Initialize display
    /// </summary>
    /// <param name="displayHardware"></param>
    void Initialize(IDisplayHardware displayHardware);

    /// <summary>
    /// Display image
    /// </summary>
    /// <param name="image"></param>
    void DisplayImage(IImage image);

    /// <summary>
    /// Return index for the closest supported color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    int GetColorIndex(Color color);

    /// <summary>
    /// Create device scan line with specified color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    byte[] CreateScanLine(Color color);

    /// <summary>
    /// Send display command
    /// </summary>
    /// <param name="command"></param>
    void SendCommand(byte command);

    /// <summary>
    /// Send display data
    /// </summary>
    /// <param name="data"></param>
    void SendData(byte[] data);

    /// <summary>
    /// Send display data
    /// </summary>
    /// <param name="stream"></param>
    void SendData(MemoryStream stream);
    #endregion

}