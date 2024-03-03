using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Color
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct Color
{

    #region Constants
    public static readonly Color Red = new(255, 0, 0);
    public static readonly Color Green = new(0, 255, 0);
    public static readonly Color Blue = new(0, 0, 255);
    public static readonly Color White = new(255, 255, 255);
    public static readonly Color Grey = new(127, 127, 127);
    public static readonly Color DarkGray = new(8, 8, 8);
    public static readonly Color Black = new(0, 0, 0);
    public static readonly Color Yellow = new(255, 255, 0);
    public static readonly Color Orange = new(255, 127, 0);
    public static readonly Color Purple = new(255, 0, 255);
    public static readonly Color Cyan = new(0, 255, 255);
    #endregion

    #region Public Fields
    /// <summary>
    /// The red component value of this instance.
    /// </summary>
    public byte R;

    /// <summary>
    /// The green component value of this instance.
    /// </summary>
    public byte G;

    /// <summary>
    /// The blue component value of this instance.
    /// </summary>
    public byte B;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="r">The red component value.</param>
    /// <param name="g">The green component value.</param>
    /// <param name="b">The blue component value.</param>
    public Color(int r, int g, int b) : this((byte)r, (byte)g, (byte)b) { }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="r">The red component value.</param>
    /// <param name="g">The green component value.</param>
    /// <param name="b">The blue component value.</param>
    public Color(byte r, byte g, byte b) => (R, G, B) = (r, g, b);
    #endregion

}