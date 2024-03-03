namespace Devices.Client.Solutions.Peripherals.EPaper.Common;

/// <summary>
/// Color
/// </summary>
public struct Color : IEquatable<Color>
{

    #region Constants
    private const double WEIGHT_RED = 0.299d;
    private const double WEIGHT_GREEN = 0.587d;
    private const double WEIGHT_BLUE = 0.114d;
    #endregion

    #region Properties
    /// <summary>
    /// Red
    /// </summary>
    public byte Red { get; private set; }

    /// <summary>
    /// Green
    /// </summary>
    public byte Green { get; private set; }

    /// <summary>
    /// Blue
    /// </summary>
    public byte Blue { get; private set; }

    /// <summary>
    /// Monochrome flag
    /// </summary>
    public readonly bool Monochrome => Red == Green && Green == Blue;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="desaturate"></param>
    public Color(byte red, byte green, byte blue, bool desaturate = false)
    {
        Red = red;
        Blue = blue;
        Green = green;
        Update(desaturate);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Desaturate color
    /// </summary>
    public void Desaturate()
    {
        if (!Monochrome)
            Red = Green = Blue = (byte)(Red * WEIGHT_RED + Green * WEIGHT_GREEN + Blue * WEIGHT_BLUE);
    }

    /// <summary>
    /// Update color
    /// </summary>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    /// <param name="desaturate"></param>
    public void Update(byte red, byte green, byte blue, bool desaturate)
    {
        Red = red;
        Blue = blue;
        Green = green;
        Update(desaturate);
    }

    /// <summary>
    /// Compare instances
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public override readonly bool Equals(object? value) => value is Color color && Equals(color);

    /// <summary>
    /// Compare instances
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public readonly bool Equals(Color value) => Red == value.Red && Green == value.Green && Blue == value.Blue;

    /// <summary>
    /// Return hash code
    /// </summary>
    /// <returns></returns>
    public override readonly int GetHashCode() => (Red, Green, Blue).GetHashCode();

    /// <summary>
    /// Equality operator
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    public static bool operator ==(Color color1, Color color2) => color1.Equals(color2);

    /// <summary>
    /// Not equals Operator
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    public static bool operator !=(Color color1, Color color2) => !color1.Equals(color2);
    #endregion

    #region Private Methods
    /// <summary>
    /// Update color
    /// </summary>
    /// <param name="desaturate"></param>
    private void Update(bool desaturate)
    {
        if (!Monochrome && desaturate)
            Desaturate();
    }
    #endregion

}