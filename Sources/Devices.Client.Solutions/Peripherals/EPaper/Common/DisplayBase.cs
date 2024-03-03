using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using System.Device.Gpio;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.EPaper.Common;

/// <summary>
/// Base display class
/// </summary>
public abstract class DisplayBase : IDisplay
{

    #region Constants
    private const int READY_TIMEOUT = 50000;
    protected const int COLOR_DISPLAY_THRESHOLD = 128;
    #endregion

    #region Private Fields
    private IDisplayWriter? displayWriter;
    private bool disposed;
    #endregion

    #region Properties
    /// <summary>
    /// Display width
    /// </summary>
    public abstract int Width { get; }

    /// <summary>
    /// Display height
    /// </summary>
    public abstract int Height { get; }

    /// <summary>
    /// Supported device colors
    /// </summary>
    public abstract Color[] SupportedColors { get; }

    /// <summary>
    /// Supported device colors
    /// </summary>
    public abstract byte[] DeviceColors { get; }

    /// <summary>
    /// Color bytes per pixel (RGB)
    /// </summary>
    public int ColorBytesPerPixel { get; set; } = 3;

    /// <summary>
    /// Monochrome flag
    /// </summary>
    public bool Monochrome { get; private set; }

    /// <summary>
    /// Sleep mode flag
    /// </summary>
    protected bool Sleeping { get; set; }

    /// <summary>
    /// Display writer
    /// </summary>
    protected IDisplayWriter DisplayWriter => displayWriter ??= GetDisplayWriter();

    /// <summary>
    /// Device pixels per byte
    /// </summary>
    public abstract int PixelsPerByte { get; }

    /// <summary>
    /// Display hardware interface
    /// </summary>
    public IDisplayHardware? DisplayHardware { get; set; }

    /// <summary>
    /// Get status command
    /// </summary>
    protected abstract byte GetStatusCommand { get; }

    /// <summary>
    /// Start data transmission command
    /// </summary>
    protected abstract byte StartDataTransmissionCommand { get; }

    /// <summary>
    /// Stop data transmission command
    /// </summary>
    protected abstract byte StopDataTransmissionCommand { get; }

    /// <summary>
    /// Deep sleep command
    /// </summary>
    protected abstract byte DeepSleepCommand { get; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <returns></returns>
    public bool WaitUntilReady() => WaitUntilReady(READY_TIMEOUT);

    /// <summary>
    /// Wait until display ready
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public bool WaitUntilReady(int timeout)
    {
        bool busy;
        var timeoutTimer = Stopwatch.StartNew();
        do
        {
            SendCommand(GetStatusCommand);
            busy = DisplayHardware?.BusyPin == PinValue.Low;
            if (timeoutTimer.ElapsedMilliseconds > timeout)
                break;
        } while (busy);
        return !busy;
    }

    /// <summary>
    /// Power controller on (do not use with sleep mode)
    /// </summary>
    public abstract void PowerOn();

    /// <summary>
    /// Power controller off (do not use with sleep mode)
    /// </summary>
    public abstract void PowerOff();

    /// <summary>
    /// Enter sleep mode
    /// </summary>
    public virtual void Sleep()
    {
        if (!Sleeping)
        {
            PowerOff();
            SendCommand(DeepSleepCommand);
            SendData(0xA5);
            Sleeping = true;
        }
    }

    /// <summary>
    /// Wake up from sleep mode
    /// </summary>
    public void WakeUp()
    {
        InitializeDevice();
        Sleeping = false;
    }

    /// <summary>
    /// Clear display to white
    /// </summary>
    public abstract void Clear();

    /// <summary>
    /// Clear display to black
    /// </summary>
    public abstract void ClearBlack();

    /// <summary>
    /// Reset display
    /// </summary>
    public void Reset()
    {
        DisplayHardware!.ResetPin = PinValue.High;
        Thread.Sleep(200);
        DisplayHardware.ResetPin = PinValue.Low;
        Thread.Sleep(4);
        DisplayHardware.ResetPin = PinValue.High;
        Thread.Sleep(200);
    }

    /// <summary>
    /// Initialize display
    /// </summary>
    /// <param name="displayHardware"></param>
    public void Initialize(IDisplayHardware displayHardware)
    {
        DisplayHardware = displayHardware;
        InitializeDevice();
        Monochrome = true;
        foreach (var color in SupportedColors)
            if (!color.Monochrome)
            {
                Monochrome = false;
                break;
            }
    }

    /// <summary>
    /// Display image
    /// </summary>
    /// <param name="image">Bitmap that should be displayed</param>
    public void DisplayImage(IImage image)
    {
        SendCommand(StartDataTransmissionCommand);
        SendBitmapToDevice(image.ScanLine, image.Stride, image.Width, image.Height);
        if (StopDataTransmissionCommand < byte.MaxValue)
            SendCommand(StopDataTransmissionCommand);
        TurnDisplayOn();
    }

    /// <summary>
    /// Return index for the closest supported color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public int GetColorIndex(Color color)
    {
        var minDistance = GetColorDistance(color, SupportedColors[0]);
        if (minDistance < 1)
            return 0;
        var bestIndex = 0;
        for (var i = 1; i < SupportedColors.Length; i++)
        {
            var deviceColor = SupportedColors[i];
            var distance = GetColorDistance(color, deviceColor);
            if (distance <= minDistance && deviceColor.Monochrome == color.Monochrome)
            {
                minDistance = distance;
                bestIndex = i;
                if (minDistance < 1)
                    break;
            }
        }
        return bestIndex;
    }

    /// <summary>
    /// Create device scan line with specified color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    public byte[] CreateScanLine(Color color) => Enumerable.Repeat(GetPixelsPackedInOneByte(color), Width / PixelsPerByte).ToArray();

    /// <summary>
    /// Send display command
    /// </summary>
    /// <param name="command"></param>
    public void SendCommand(byte command)
    {
        DisplayHardware!.SpiDcPin = PinValue.Low;
        DisplayHardware.WriteByte(command);
    }

    /// <summary>
    /// Send display data
    /// </summary>
    /// <param name="data"></param>
    public void SendData(byte data)
    {
        DisplayHardware!.SpiDcPin = PinValue.High;
        DisplayHardware.WriteByte(data);
    }

    /// <summary>
    /// Send display data
    /// </summary>
    /// <param name="data"></param>
    public void SendData(byte[] data)
    {
        DisplayHardware!.SpiDcPin = PinValue.High;
        DisplayHardware.Write(data);
    }

    /// <summary>
    /// Send display data
    /// </summary>
    /// <param name="stream"></param>
    public void SendData(MemoryStream stream)
    {
        DisplayHardware!.SpiDcPin = PinValue.High;
        DisplayHardware.Write(stream);
    }

    /// <summary>
    /// Pack pixel bits into device byte
    /// </summary>
    /// <param name="pixels"></param>
    /// <returns></returns>
    public byte PackPixels(params byte[] pixels)
    {
        if (pixels == null || pixels.Length == 0)
            throw new ArgumentException($"Argument {nameof(pixels)} can not be null or empty.", nameof(pixels));
        if (pixels.Length != PixelsPerByte)
            throw new ArgumentException($"Argument {nameof(pixels)}.Length is not PixelsPerByte {PixelsPerByte}.", nameof(pixels));
        var bitMoveLength = 8 / PixelsPerByte;
        var maxValue = (byte)Math.Pow(2, bitMoveLength) - 1;
        byte output = 0;
        for (var i = 0; i < pixels.Length; i++)
        {
            var bitMoveValue = 8 - bitMoveLength - (i * bitMoveLength);
            var value = (byte)(pixels[i] << bitMoveValue);
            var mask = maxValue << bitMoveValue;
            var posValue = (byte)(value & mask);
            output |= posValue;
        }
        return output;
    }

    /// <summary>
    /// Send bitmap as byte array to device
    /// </summary>
    /// <param name="scanLine"></param>
    /// <param name="stride"></param>
    /// <param name="maxX"></param>
    /// <param name="maxY"></param>
    public void SendBitmapToDevice(IntPtr scanLine, int stride, int maxX, int maxY)
    {
        var inputLine = new byte[stride];
        var pixel = new Color(0, 0, 0);
        for (var y = 0; y < maxY; y++, scanLine += stride)
        {
            Marshal.Copy(scanLine, inputLine, 0, inputLine.Length);
            var xPos = 0;
            for (var x = 0; x < maxX; x++)
            {
                pixel.Update(inputLine[xPos++], inputLine[xPos++], inputLine[xPos++], Monochrome);
                if (ColorBytesPerPixel > 3)
                    xPos += ColorBytesPerPixel - 3;
                DisplayWriter.Write(GetColorIndex(pixel));
            }
            for (var x = maxX; x < Width; x++)
                DisplayWriter.WriteBlankPixel();
        }
        for (var y = maxY; y < Height; y++)
            DisplayWriter.WriteBlankLine();
        DisplayWriter.Finish();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Gets Euclidean distance between two colors
    /// </summary>
    /// <param name="color1"></param>
    /// <param name="color2"></param>
    /// <returns></returns>
    protected double GetColorDistance(Color color1, Color color2)
    {
        if (Monochrome)
            return (color1.Red - color2.Red) * (color1.Red - color2.Red);
        var (y1, u1, v1) = GetYuv(color1);
        var (y2, u2, v2) = GetYuv(color2);
        var diffY = y1 - y2;
        var diffU = u1 - u2;
        var diffV = v1 - v2;
        return diffY * diffY + diffU * diffU + diffV * diffV;
    }

    /// <summary>
    /// Initialize device
    /// </summary>
    protected abstract void InitializeDevice();

    /// <summary>
    /// Turn display on after sleep
    /// </summary>
    protected abstract void TurnDisplayOn();

    /// <summary>
    /// Convert color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    protected abstract byte ToByte(Color color);

    /// <summary>
    /// Return display writer
    /// </summary>
    /// <returns>Returns a display writer</returns>
    protected virtual IDisplayWriter GetDisplayWriter() => new DisplayWriter(this);
    #endregion

    #region Private Methods
    /// <summary>
    /// Shutdown device
    /// </summary>
    private void ShutdownDevice()
    {
        if (DisplayHardware != null)
        {
            Sleep();
            DisplayHardware.Dispose();
            DisplayHardware = null;
        }
    }

    /// <summary>
    /// Return single byte with device pixels
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private byte GetPixelsPackedInOneByte(Color color) => PackPixels(Enumerable.Repeat(ToByte(color), PixelsPerByte).ToArray());

    /// <summary>
    /// Calculate YUV color space
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static (double Y, double U, double V) GetYuv(Color color) => (color.Red * .299000 + color.Green * .587000 + color.Blue * .114000, color.Red * -.168736 + color.Green * -.331264 + color.Blue * .500000 + 128, color.Red * .500000 + color.Green * -.418688 + color.Blue * -.081312 + 128);
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    ~DisplayBase() => Dispose(false);

    /// <summary>
    /// Finalization
    /// </summary>
    public virtual void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing">Explicit dispose</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                displayWriter?.Dispose();
                ShutdownDevice();
            }
            disposed = true;
        }
    }
    #endregion

}