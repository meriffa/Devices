using Devices.Client.Solutions.Peripherals.EPaper.Common;
using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

namespace Devices.Client.Solutions.Peripherals.EPaper.Devices;

/// <summary>
/// Waveshare 7.5inch e-Paper (B) (800x480, Black, White, Red) Writer
/// </summary>
public sealed class Waveshare75BWriter : DisplayWriter
{

    #region Private Fields
    private readonly int redIndex;
    private readonly byte redPixel;
    private readonly int blankIndex;
    private readonly byte blankPixel;
    private readonly byte[] blankLine;
    private MemoryStream? memoryStream = new();
    private byte output;
    private int pixelCount = -1;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="display"></param>
    public Waveshare75BWriter(IDisplay display) : base(display)
    {
        redPixel = display.DeviceColors[redIndex = display.GetColorIndex(Colors.Red)];
        blankPixel = display.DeviceColors[blankIndex = display.GetColorIndex(Colors.Black)];
        blankLine = display.CreateScanLine(Colors.Black);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Write buffer data
    /// </summary>
    /// <param name="index"></param>
    public override void Write(int index)
    {
        byte value;
        base.Write(index == redIndex ? blankIndex : index);
        value = index == redIndex ? redPixel : blankPixel;
        pixelCount++;
        if (PixelsPerByte == 1)
            memoryStream?.WriteByte(value);
        else
        {
            output <<= BitShift;
            output |= value;
            if (pixelCount % PixelsPerByte == PixelThreshold)
            {
                memoryStream?.WriteByte(output);
                output = 0;
            }
        }
    }

    /// <summary>
    /// Write blank line
    /// </summary>
    public override void WriteBlankLine()
    {
        base.WriteBlankLine();
        while (PixelsPerByte > 1 && pixelCount % PixelsPerByte != PixelThreshold)
            Write(BlankIndex);
        memoryStream?.Write(blankLine, 0, blankLine.Length);
    }

    /// <summary>
    /// Send buffer data to display
    /// </summary>
    public override void Finish()
    {
        base.Finish();
        if (memoryStream?.Length > 0)
        {
            Display.SendCommand((byte)Waveshare75B.Commands.DataStartTransmission2);
            memoryStream.Position = 0;
            var buffer = new byte[StreamWriteThreshold];
            var bytesRead = -1;
            while (bytesRead != 0)
                if ((bytesRead = memoryStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    if (bytesRead != buffer.Length)
                        Array.Resize(ref buffer, bytesRead);
                    Display.SendData(buffer);
                }
        }
        output = 0;
        pixelCount = -1;
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing && memoryStream != null)
        {
            Finish();
            memoryStream.Close();
            memoryStream.Dispose();
            memoryStream = null;
        }
    }
    #endregion

}