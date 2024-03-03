using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

namespace Devices.Client.Solutions.Peripherals.EPaper.Common;

/// <summary>
/// Display writer
/// </summary>
public class DisplayWriter : IDisplayWriter
{

    #region Private Fields
    private readonly byte[] blankLine;
    private MemoryStream? memoryStream = new();
    private byte output;
    private int pixelCount = -1;
    #endregion

    #region Properties
    /// <summary>
    /// Display device
    /// </summary>
    protected IDisplay Display { get; }

    protected int StreamWriteThreshold { get; private set; }

    /// <summary>
    /// Device pixels per byte
    /// </summary>
    protected int PixelsPerByte { get; }

    /// <summary>
    /// Pixel threshold
    /// </summary>
    protected int PixelThreshold { get; }

    /// <summary>
    /// Bit shift value
    /// </summary>
    protected byte BitShift { get; }

    /// <summary>
    /// Pallet color index for blank pixel
    /// </summary>
    protected int BlankIndex { get; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="display"></param>
    public DisplayWriter(IDisplay display)
    {
        Display = display;
        StreamWriteThreshold = display.Width / display.PixelsPerByte;
        PixelsPerByte = display.PixelsPerByte;
        BitShift = (byte)(8 / PixelsPerByte);
        PixelThreshold = PixelsPerByte - 1;
        BlankIndex = display.GetColorIndex(Colors.White);
        blankLine = display.CreateScanLine(Colors.White);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Write buffer data
    /// </summary>
    /// <param name="index"></param>
    public virtual void Write(int index)
    {
        var value = Display.DeviceColors[index];
        pixelCount++;
        if (PixelsPerByte == 1)
        {
            memoryStream?.WriteByte(value);
            if (memoryStream?.Length >= StreamWriteThreshold)
                Flush();
        }
        else
        {
            output <<= BitShift;
            output |= value;
            if (pixelCount % PixelsPerByte == PixelThreshold)
            {
                memoryStream?.WriteByte(output);
                output = 0;
                if (memoryStream?.Length >= StreamWriteThreshold)
                    Flush();
            }
        }
    }

    /// <summary>
    /// Write blank pixel
    /// </summary>
    public virtual void WriteBlankPixel() => Write(BlankIndex);

    /// <summary>
    /// Write blank line
    /// </summary>
    public virtual void WriteBlankLine()
    {
        while (PixelsPerByte > 1 && pixelCount % PixelsPerByte != PixelThreshold)
            Write(BlankIndex);
        memoryStream?.Write(blankLine, 0, blankLine.Length);
        Flush();
    }

    /// <summary>
    /// Send buffer data to display
    /// </summary>
    public virtual void Finish()
    {
        Flush();
        output = 0;
        pixelCount = -1;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Send display data & stream clear
    /// </summary>
    protected virtual void Flush()
    {
        if (memoryStream?.Length > 0)
        {
            memoryStream.Position = 0;
            Display.SendData(memoryStream);
            memoryStream.SetLength(0);
        }
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing && memoryStream != null)
        {
            Finish();
            memoryStream.Close();
            memoryStream.Dispose();
            memoryStream = null;
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
    ~DisplayWriter() => Dispose(false);
    #endregion

}