using Devices.Client.Solutions.Peripherals.EPaper.Common;
using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Devices.Client.Solutions.Peripherals.EPaper.Images.ImageSharp;

/// <summary>
/// SixLabors.ImageSharp display factory
/// </summary>
/// <typeparam name="TPixel"></typeparam>
public static class DisplayFactory<TPixel> where TPixel : unmanaged, IPixel<TPixel>
{

    #region Public Methods
    /// <summary>
    /// Create display instance
    /// </summary>
    /// <param name="displayType"></param>
    /// <returns></returns>
    public static IImageDevice<Image<TPixel>> Create(DisplayType displayType) => new BitmapLoader<TPixel>(DisplayFactory.Create(displayType));
    #endregion

}