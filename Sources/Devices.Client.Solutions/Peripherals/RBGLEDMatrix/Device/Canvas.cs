namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// Canvas
/// </summary>
public class Canvas
{

    #region Properties
    internal IntPtr Handle { get; set; }
    #endregion

    #region Properties
    /// <summary>
    /// The width of the canvas in pixels.
    /// </summary>
    public int Width { get; private set; }

    /// <summary>
    /// The height of the canvas in pixels.
    /// </summary>
    public int Height { get; private set; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="handle"></param>
    internal Canvas(IntPtr handle)
    {
        Handle = handle;
        MatrixLibrary.GetSize(handle, out var width, out var height);
        Width = width;
        Height = height;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Sets the color of a specific pixel.
    /// </summary>
    /// <param name="x">The X coordinate of the pixel.</param>
    /// <param name="y">The Y coordinate of the pixel.</param>
    /// <param name="color">New pixel color.</param>
    public void SetPixel(int x, int y, Color color) => MatrixLibrary.SetPixel(Handle, x, y, color.R, color.G, color.B);

    /// <summary>
    /// Copies the colors from the specified buffer to a rectangle on the canvas.
    /// </summary>
    /// <param name="x">The X coordinate of the top-left pixel of the rectangle.</param>
    /// <param name="y">The Y coordinate of the top-left pixel of the rectangle.</param>
    /// <param name="width">Width of the rectangle.</param>
    /// <param name="height">Height of the rectangle.</param>
    /// <param name="colors">Buffer containing the colors to copy.</param>
    public void SetPixels(int x, int y, int width, int height, Span<Color> colors)
    {
        if (colors.Length < width * height)
            throw new ArgumentOutOfRangeException(nameof(colors));
        MatrixLibrary.SetPixels(Handle, x, y, width, height, ref colors[0]);
    }

    /// <summary>
    /// Sets an image from the given buffer containing pixels.
    /// </summary>
    /// <param name="x">Image offset X</param>
    /// <param name="y">Image offset Y</param>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="colors">Image buffer (The buffer needs to be organized as rows with columns of three bytes organized as RGB. The size of the buffer needs to be exactly 3 * width * height bytes.</param>
    public void SetPixels(int x, int y, int width, int height, byte[] colors) => MatrixLibrary.SetImage(Handle, x, y, colors, (uint)colors.Length, width, height, 0);

    /// <summary>
    /// Sets the color of the entire canvas.
    /// </summary>
    /// <param name="color">New canvas color.</param>
    public void Fill(Color color) => MatrixLibrary.Fill(Handle, color.R, color.G, color.B);

    /// <summary>
    /// Cleans the entire canvas.
    /// </summary>
    public void Clear() => MatrixLibrary.Clear(Handle);

    /// <summary>
    /// Draws a line of the specified color.
    /// </summary>
    /// <param name="x0">The X coordinate of the first point.</param>
    /// <param name="y0">The Y coordinate of the first point.</param>
    /// <param name="x1">The X coordinate of the second point.</param>
    /// <param name="y1">The Y coordinate of the second point.</param>
    /// <param name="color">The color of the line.</param>
    public void DrawLine(int x0, int y0, int x1, int y1, Color color) => MatrixLibrary.DrawLine(Handle, x0, y0, x1, y1, color.R, color.G, color.B);

    /// <summary>
    /// Draws a circle of the specified color.
    /// </summary>
    /// <param name="x">The X coordinate of the center.</param>
    /// <param name="y">The Y coordinate of the center.</param>
    /// <param name="radius">The radius of the circle, in pixels.</param>
    /// <param name="color">The color of the circle.</param>
    public void DrawCircle(int x, int y, int radius, Color color) => MatrixLibrary.DrawCircle(Handle, x, y, radius, color.R, color.G, color.B);

    /// <summary>
    /// Draws the text with the specified color.
    /// </summary>
    /// <param name="font">Font to draw text with.</param>
    /// <param name="x">The X coordinate of the starting point.</param>
    /// <param name="y">The Y coordinate of the starting point.</param>
    /// <param name="color">The color of the text.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="spacing">Additional spacing between characters.</param>
    /// <param name="vertical">Whether to draw the text vertically.</param>
    /// <returns>How many pixels was advanced on the screen.</returns>
    public int DrawText(Font font, int x, int y, Color color, string text, int spacing = 0, bool vertical = false) => font.DrawText(Handle, x, y, color, text, spacing, vertical);
    #endregion

}