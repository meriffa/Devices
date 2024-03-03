using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using System.Device.Gpio;
using System.Device.Spi;

namespace Devices.Client.Solutions.Peripherals.EPaper.Common;

/// <summary>
/// Display hardware
/// </summary>
public sealed class DisplayHardware : IDisplayHardware
{

    #region Constants
    private const int RESET_PIN = 17;
    private const int SPI_DC_PIN = 25;
    private const int BUSY_PIN = 24;
    private const int POWER_PIN = 18;
    #endregion

    #region Properties
    /// <summary>
    /// SPI bus device
    /// </summary>
    public SpiDevice? SpiDevice { get; private set; }

    /// <summary>
    /// GPIO controller
    /// </summary>
    public GpioController? GpioController { get; private set; }

    /// <summary>
    /// GPIO reset pin
    /// </summary>
    public PinValue ResetPin { get => GpioController!.Read(RESET_PIN); set => GpioController?.Write(RESET_PIN, value); }

    /// <summary>
    /// GPIO SPI DC pin
    /// </summary>
    public PinValue SpiDcPin { get => GpioController!.Read(SPI_DC_PIN); set => GpioController?.Write(SPI_DC_PIN, value); }

    /// <summary>
    /// GPIO Busy pin
    /// </summary>
    public PinValue BusyPin { get => GpioController!.Read(BUSY_PIN); set => GpioController?.Write(BUSY_PIN, value); }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public DisplayHardware() : this(CreateSpiDevice(), CreateGpioController())
    {
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="spiDevice"></param>
    /// <param name="gpioController"></param>
    public DisplayHardware(SpiDevice spiDevice, GpioController gpioController)
    {
        SpiDevice = spiDevice;
        GpioController = gpioController;
        GpioController?.OpenPin(RESET_PIN, PinMode.Output);
        GpioController?.OpenPin(SPI_DC_PIN, PinMode.Output);
        GpioController?.OpenPin(POWER_PIN, PinMode.Output);
        GpioController?.OpenPin(BUSY_PIN, PinMode.Input);
        GpioController?.Write(POWER_PIN, PinValue.High);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Write data to SPI device
    /// </summary>
    /// <param name="stream"></param>
    public void Write(MemoryStream stream)
    {
        byte[] buffer = new byte[Math.Min(4096, stream.Length)];
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        while (bytesRead == buffer.Length)
        {
            SpiDevice?.Write(buffer);
            bytesRead = stream.Read(buffer, 0, buffer.Length);
        }
        if (bytesRead > 0 && bytesRead < buffer.Length)
        {
            Array.Resize(ref buffer, bytesRead);
            SpiDevice?.Write(buffer);
        }
    }

    /// <summary>
    /// Write data to SPI device
    /// </summary>
    /// <param name="buffer"></param>
    public void Write(byte[] buffer) => SpiDevice?.Write(buffer);

    /// <summary>
    /// Write a byte to SPI device
    /// </summary>
    /// <param name="value"></param>
    public void WriteByte(byte value) => SpiDevice?.WriteByte(value);
    #endregion

    #region Private Methods
    /// <summary>
    /// Create GPIO controller
    /// </summary>
    /// <returns></returns>
    private static GpioController CreateGpioController() => new();

    /// <summary>
    /// Create SPI device
    /// </summary>
    /// <returns></returns>
    private static SpiDevice CreateSpiDevice() => SpiDevice.Create(new SpiConnectionSettings(0, 0) { ClockFrequency = 4000000 });
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    private void Dispose(bool disposing)
    {
        if (disposing)
        {
            GpioController?.Write(POWER_PIN, PinValue.Low);
            GpioController?.Write(SPI_DC_PIN, PinValue.Low);
            GpioController?.Write(RESET_PIN, PinValue.Low);
            GpioController?.Dispose();
            GpioController = null;
            SpiDevice?.Dispose();
            SpiDevice = null;
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
    ~DisplayHardware() => Dispose(false);
    #endregion

}