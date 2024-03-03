using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace Devices.Client.Solutions.Peripherals.EPaper.Images.ImageSharp;

/// <summary>
/// SixLabors.ImageSharp.Image bitmap image
/// </summary>
/// <typeparam name="TPixel"></typeparam>
public sealed class BitmapImage<TPixel> : IImage where TPixel : unmanaged, IPixel<TPixel>
{

    #region Private Fields
    private Image<TPixel>? bitmap;
    private readonly Memory<TPixel> bitmapData;
    private MemoryHandle? bitmapHandle;
    private IntPtr scanLine;
    #endregion

    #region Properties
    /// <summary>
    /// Image width
    /// </summary>
    public int Width => bitmap?.Width ?? 0;

    /// <summary>
    /// Image height
    /// </summary>
    public int Height => bitmap?.Height ?? 0;

    /// <summary>
    /// Scan line length in bytes
    /// </summary>
    public int Stride => bitmap?.Width * Unsafe.SizeOf<TPixel>() ?? 0;

    /// <summary>
    /// Bytes per pixel
    /// </summary>
    public int BytesPerPixel => Unsafe.SizeOf<TPixel>();

    /// <summary>
    /// Image byte array pointer
    /// </summary>
    public unsafe IntPtr ScanLine
    {
        get
        {
            if (scanLine == IntPtr.Zero && bitmap != null)
            {
                bitmapHandle = bitmapData.Pin();
                scanLine = (IntPtr)bitmapHandle.Value.Pointer;
            }
            return scanLine;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="bitmap"></param>
    /// <param name="maxWidth"></param>
    /// <param name="maxHeight"></param>
    public BitmapImage(Image<TPixel> bitmap, int maxWidth, int maxHeight)
    {
        if (typeof(TPixel) != typeof(Rgb24) && typeof(TPixel) != typeof(Rgba32))
            throw new("Bitmap pixel type is not supported.");
        this.bitmap = bitmap;
        if (bitmap.Width != maxWidth || bitmap.Height != maxHeight)
            bitmap.Mutate(o => o.Resize(maxWidth, maxHeight));
        if (!bitmap.DangerousTryGetSinglePixelMemory(out bitmapData))
            throw new("Pixel buffer read failed.");
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        bitmapHandle?.Dispose();
        bitmapHandle = null;
        bitmap?.Dispose();
        bitmap = null;
    }
    #endregion

}