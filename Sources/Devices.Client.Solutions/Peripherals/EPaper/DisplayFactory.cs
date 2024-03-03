using Devices.Client.Solutions.Peripherals.EPaper.Common;
using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

namespace Devices.Client.Solutions.Peripherals.EPaper;

/// <summary>
/// Display factory
/// </summary>
public static class DisplayFactory
{

    #region Properties
    /// <summary>
    /// Display hardware
    /// </summary>
    public static Lazy<IDisplayHardware> DisplayHardware { get; set; } = new Lazy<IDisplayHardware>(() => new DisplayHardware());
    #endregion

    #region Public Methods
    /// <summary>
    /// Create display instance
    /// </summary>
    /// <param name="displayType"></param>
    /// <returns></returns>
    public static IDisplay Create(DisplayType displayType)
    {
        IDisplay display = displayType switch
        {
            DisplayType.Waveshare565F => new Devices.Waveshare565F(),
            DisplayType.Waveshare75C => new Devices.Waveshare75C(),
            DisplayType.Waveshare75 => new Devices.Waveshare75(),
            DisplayType.Waveshare75B => new Devices.Waveshare75B(),
            _ => throw new($"Display type '{displayType}' is not supported.")
        };
        display.Initialize(DisplayHardware.Value);
        return display;
    }
    #endregion

}