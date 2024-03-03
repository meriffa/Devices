namespace Devices.Client.Solutions.Peripherals.EPaper.Common;

/// <summary>
/// Display type
/// </summary>
public enum DisplayType
{
    None,
    Waveshare565F,      // Waveshare 5.65inch e-Paper (F) (600x448, Black, White, Green, Blue, Red, Yellow, Orange)
    Waveshare75C,       // WaveShare 7.5inch e-Paper (C)  (640x384, Black, White, Yellow)
    Waveshare75,        // Waveshare 7.5inch e-Paper      (800x480, Black, White)
    Waveshare75B        // Waveshare 7.5inch e-Paper (B)  (800x480, Black, White, Red)
}