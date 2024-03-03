namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// BDF Font
/// </summary>
public class Font : IDisposable
{

    #region Private Fields
    private readonly IntPtr handle;
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// Return font height
    /// </summary>
    /// <returns></returns>
    public int Height => MatrixLibrary.GetFontHeight(handle);

    /// <summary>
    /// Return font baseline
    /// </summary>
    /// <returns></returns>
    public int Baseline => MatrixLibrary.GetFontBaseline(handle);
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="path">The path to the BDF file to load.</param>
    public Font(string path)
    {
        handle = MatrixLibrary.LoadFont(path);
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="handle"></param>
    private Font(IntPtr handle)
    {
        this.handle = handle;
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Return outline font
    /// </summary>
    /// <returns></returns>
    public Font CreateOutlineFont() => new(MatrixLibrary.CreateOutlineFont(handle));

    /// <summary>
    /// Draw text
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    /// <param name="text"></param>
    /// <param name="spacing"></param>
    /// <param name="vertical"></param>
    /// <returns></returns>
    public int DrawText(IntPtr canvas, int x, int y, Color color, string text, int spacing = 0, bool vertical = false)
    {
        if (!vertical)
            return MatrixLibrary.DrawTextHorizontal(canvas, handle, x, y, color.R, color.G, color.B, text, spacing);
        else
            return MatrixLibrary.DrawTextVertical(canvas, handle, x, y, color.R, color.G, color.B, text, spacing);
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            MatrixLibrary.DeleteFont(handle);
            disposed = true;
        }
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
    /// Finalization
    /// </summary>
    ~Font() => Dispose(false);
    #endregion

}