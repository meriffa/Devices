using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// Matrix
/// </summary>
public class Matrix : IDisposable
{

    #region Private Fields
    private readonly IntPtr handle;
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// The general brightness of the matrix.
    /// </summary>
    public byte Brightness
    {
        get => MatrixLibrary.GetBrightness(handle);
        set => MatrixLibrary.SetBrightness(handle, value);
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Initializes a new matrix.
    /// </summary>
    /// <param name="options">A configuration of a matrix.</param>
    public Matrix(MatrixOptions options)
    {
        MatrixOptionsDevice optionsInternal = new(options);
        try
        {
            var args = new string[] { AppDomain.CurrentDomain.FriendlyName, "--led-no-drop-privs" };
            handle = MatrixLibrary.CreateMatrixFromOptions(ref optionsInternal, args.Length, args);
            if (handle == 0)
                throw new ArgumentException("Could not initialize a new matrix.");
        }
        finally
        {
            Marshal.FreeHGlobal(optionsInternal.hardware_mapping);
            if (options.RGBSequence is not null)
                Marshal.FreeHGlobal(optionsInternal.led_rgb_sequence);
            if (options.PixelMapper is not null)
                Marshal.FreeHGlobal(optionsInternal.pixel_mapper_config);
            if (options.PanelType is not null)
                Marshal.FreeHGlobal(optionsInternal.panel_type);
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Returns a canvas representing the current frame buffer.
    /// </summary>
    /// <returns>An instance of <see cref="Canvas"/> representing the canvas.</returns>
    /// <remarks>Consider using <see cref="CreateOffscreenCanvas"/> instead.</remarks>
    public Canvas GetCanvas() => new(MatrixLibrary.GetCanvas(handle));

    /// <summary>
    /// Creates a new back-buffer canvas for drawing on.
    /// </summary>
    /// <returns>An instance of <see cref="Canvas"/> representing the canvas.</returns>
    public Canvas CreateOffscreenCanvas() => new(MatrixLibrary.CreateOffscreenCanvas(handle));

    /// <summary>
    /// Swaps this canvas with the currently active canvas. The active canvas becomes a back buffer and is mapped to <paramref name="canvas"/> instance. This operation guarantees vertical synchronization.
    /// </summary>
    /// <param name="canvas">Backbuffer canvas to swap.</param>
    public void SwapOnVSync(Canvas canvas) => canvas.Handle = MatrixLibrary.SwapOnVSync(handle, canvas.Handle);
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
            MatrixLibrary.DeleteMatrix(handle);
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
    ~Matrix() => Dispose(false);
    #endregion

}