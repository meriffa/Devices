using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Devices.Client.Solutions.Peripherals.EPaper.Images.ImageSharp;

/// <summary>
/// SixLabors.ImageSharp.Image loader
/// </summary>
/// <typeparam name="TPixel"></typeparam>
/// <param name="display"></param>
public sealed class BitmapLoader<TPixel>(IDisplay display) : ImageDevice<Image<TPixel>>(display) where TPixel : unmanaged, IPixel<TPixel>
{

    #region Protected Methods
    /// <summary>
    /// Load image
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    protected override IImage LoadImage(Image<TPixel> image) => new BitmapImage<TPixel>(image, Math.Min(Width, image.Width), Math.Min(Height, image.Height));
    #endregion

}