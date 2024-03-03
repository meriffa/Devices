namespace Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

/// <summary>
/// Image device interface
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IImageDevice<T> : IDevice
{

    #region Public Methods
    /// <summary>
    /// Display image
    /// </summary>
    /// <param name="image"></param>
    void DisplayImage(T image);
    #endregion

}