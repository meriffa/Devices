namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Image interface
/// </summary>
public interface IImage : IDisposable
{

    #region Properties
    /// <summary>
    /// Image width
    /// </summary>
    int Width { get; }

    /// <summary>
    /// Image height
    /// </summary>
    int Height { get; }

    /// <summary>
    /// Scan line length in bytes
    /// </summary>
    int Stride { get; }

    /// <summary>
    /// Bytes per pixel
    /// </summary>
    int BytesPerPixel { get; }

    /// <summary>
    /// Image byte array pointer
    /// </summary>
    IntPtr ScanLine { get; }
    #endregion

}