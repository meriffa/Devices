namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Point
/// </summary>
public class Point
{

    #region Properties
    /// <summary>
    /// Point X
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Point Y
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Recycled flag
    /// </summary>
    public bool Recycled { get; set; } = false;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public Point(int x, int y) => (X, Y) = (x, y);
    #endregion

}