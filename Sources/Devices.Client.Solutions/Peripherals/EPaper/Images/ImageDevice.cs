using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

namespace Devices.Client.Solutions.Peripherals.EPaper.Images;

/// <summary>
/// Base image device class
/// </summary>
/// <typeparam name="T"></typeparam>
/// <param name="display"></param>
public abstract class ImageDevice<T>(IDisplay display) : IImageDevice<T>
{

    #region Properties
    /// <summary>
    /// Display
    /// </summary>
    private IDisplay? Display { get; set; } = display;

    /// <summary>
    /// Display width
    /// </summary>
    public int Width => Display!.Width;

    /// <summary>
    /// Display height
    /// </summary>
    public int Height => Display!.Height;
    #endregion

    #region Public Methods
    /// <summary>
    /// Display image
    /// </summary>
    /// <param name="image"></param>
    public void DisplayImage(T image)
    {
        using var rawImage = LoadImage(image);
        Display!.ColorBytesPerPixel = rawImage.BytesPerPixel;
        Display.DisplayImage(rawImage);
    }

    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <returns></returns>
    public bool WaitUntilReady() => Display!.WaitUntilReady();

    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool WaitUntilReady(int timeout) => Display!.WaitUntilReady(timeout);

    /// <summary>
    /// Power controller on (do not use with sleep mode)
    /// </summary>
    public void PowerOn() => Display!.PowerOn();

    /// <summary>
    /// Power controller off (do not use with sleep mode)
    /// </summary>
    public void PowerOff() => Display!.PowerOff();

    /// <summary>
    /// Enter sleep mode
    /// </summary>
    public void Sleep() => Display!.Sleep();

    /// <summary>
    /// Wake up from sleep mode
    /// </summary>
    public void WakeUp() => Display!.WakeUp();

    /// <summary>
    /// Clear display to white
    /// </summary>
    public void Clear() => Display!.Clear();

    /// <summary>
    /// Clear display to black
    /// </summary>
    public void ClearBlack() => Display!.ClearBlack();

    /// <summary>
    /// Reset display
    /// </summary>
    public void Reset() => Display!.Reset();
    #endregion

    #region Protected Methods
    /// <summary>
    /// Load image
    /// </summary>
    /// <param name="image"></param>
    /// <returns></returns>
    protected abstract IImage LoadImage(T image);
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    ~ImageDevice()
    {
        Dispose(false);
    }

    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Dispose for sub classes
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        Display?.Dispose();
        Display = null;
    }
    #endregion

}