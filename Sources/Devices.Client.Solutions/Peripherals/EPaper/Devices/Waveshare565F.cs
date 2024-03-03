using Devices.Client.Solutions.Peripherals.EPaper.Common;

namespace Devices.Client.Solutions.Peripherals.EPaper.Devices;

/// <summary>
/// Waveshare 5.65inch e-Paper (F) (600x448, Black, White, Green, Blue, Red, Yellow, Orange)
/// </summary>
public sealed class Waveshare565F : DisplayBase
{

    #region Colors
    /// <summary>
    /// Hardware colors
    /// </summary>
    private enum HardwareColors
    {
        Black = 0x00,
        White = 0x01,
        Green = 0x02,
        Blue = 0x03,
        Red = 0x04,
        Yellow = 0x05,
        Orange = 0x06,
        Clean = 0x07
    }
    #endregion

    #region Commands
    /// <summary>
    /// Hardware commands
    /// </summary>
    private enum Commands
    {
        None = -1,
        PanelSetting = 0x00, // Panel Setting (PSR) (R00H)
        PowerSetting = 0x01, // Power Setting (PWR) (R01H)
        PowerOff = 0x02, // Power OFF (POF) (R02H)
        PowerOffSequenceSetting = 0x03, // Power OFF Sequence Setting (PFS) (R03H)
        PowerOn = 0x04, // Power ON (PON) (R04H)
        BoosterSoftStart = 0x06, // Booster Soft Start (BTST) (R06H)
        DeepSleep = 0x07, // Deep Sleep (DSLP) (R07H)
        DataStartTransmission1 = 0x10, // Data Start Transmission 1 (DTM1) (R10H)
        DataStop = 0x11, // Data Stop Transmission (DSP) (R11H)
        DisplayRefresh = 0x12, // Display Refresh (DRF) (R12H)
        ImageProcess = 0x13, // Image Process Command (IPC) (R13H)
        PllControl = 0x30, // PLL Control (PLL) (R30H)
        TemperatureSensorCommand = 0x40, // Temperature Sensor Calibration (TSC) (R40H)
        TemperatureCalibration = 0x41, // Temperature Sensor Internal/External (TSE) (R41H)
        TemperatureSensorWrite = 0x42, // Temperature Sensor Write (TSW) (R42H)
        TemperatureSensorRead = 0x43, // Temperature Sensor Read (TSR) (R43H)
        VcomAndDataIntervalSetting = 0x50, // VCOM and Data Interval Setting (CDI) (R50H)
        LowPowerDetection = 0x51, // Low Power Detection(LPD) (R51h)
        TconSetting = 0x60, // TCON Setting (TCON) (R60h) Undocumented
        TconResolution = 0x61, // Resolution Setting (TRES) (R61H)
        GetStatus = 0x71, // Get Status (FLG) (R71H)
        ReadVcomValue = 0x81, // VCOM Value (VV) (R81h)
        VcmDcSetting = 0x82, // VCOM-DC Setting (VDCS) (R82H)
        FlashMode = 0xE3 // Flash Mode (RE5H) Undocumented
    }
    #endregion

    #region Properties
    /// <summary>
    /// Display width
    /// </summary>
    public override int Width { get; } = 600;

    /// <summary>
    /// Display height
    /// </summary>
    public override int Height { get; } = 448;

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override Color[] SupportedColors { get; } = [Colors.Black, Colors.White, Colors.Green, Colors.Blue, Colors.Red, Colors.Yellow, Colors.Orange];

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override byte[] DeviceColors { get; } = [(byte)HardwareColors.Black, (byte)HardwareColors.White, (byte)HardwareColors.Green, (byte)HardwareColors.Blue, (byte)HardwareColors.Red, (byte)HardwareColors.Yellow, (byte)HardwareColors.Orange];

    /// <summary>
    /// Device pixels per byte
    /// </summary>
    public override int PixelsPerByte { get; } = 2;

    /// <summary>
    /// Get status command
    /// </summary>
    protected override byte GetStatusCommand { get; } = (byte)Commands.GetStatus;

    /// <summary>
    /// Start data transmission command
    /// </summary>
    protected override byte StartDataTransmissionCommand { get; } = (byte)Commands.DataStartTransmission1;

    /// <summary>
    /// Stop data transmission command
    /// </summary>
    protected override byte StopDataTransmissionCommand { get; } = (byte)Commands.DataStop;

    /// <summary>
    /// Deep sleep command
    /// </summary>
    protected override byte DeepSleepCommand { get; } = (byte)Commands.DeepSleep;
    #endregion

    #region Public Methods
    /// <summary>
    /// Clear display to white
    /// </summary>
    public override void Clear()
    {
        FillColor(Colors.White);
        TurnDisplayOn();
    }

    /// <summary>
    /// Clear display to black
    /// </summary>
    public override void ClearBlack()
    {
        FillColor(Colors.Black);
        TurnDisplayOn();
    }

    /// <summary>
    /// Power controller on (do not use with sleep mode)
    /// </summary>
    public override void PowerOn()
    {
        SendCommand(Commands.PowerOn);
        WaitUntilReady();
    }

    /// <summary>
    /// Power controller off (do not use with sleep mode)
    /// </summary>
    public override void PowerOff()
    {
        SendCommand(Commands.PowerOff);
        WaitUntilReady();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Initialize device
    /// </summary>
    protected override void InitializeDevice()
    {
        Reset();
        SendCommand(Commands.PanelSetting); // POWER_SETTING
        SendData(0xEF);
        SendData(0x08);
        SendCommand(Commands.PowerSetting);
        SendData(0x37);
        SendData(0x00);
        SendData(0x23);
        SendData(0x23);
        SendCommand(Commands.PowerOffSequenceSetting);
        SendData(0x00);
        SendCommand(Commands.PowerOn);
        SendData(0xC7);
        SendData(0xC7);
        SendData(0x1D);
        SendCommand(Commands.PllControl);
        SendData(0x3C);
        SendCommand(Commands.TemperatureCalibration);
        SendData(0x00);
        SendCommand(Commands.VcomAndDataIntervalSetting);
        SendData(0x37);
        SendCommand(Commands.TconSetting);
        SendData(0x22);
        SendCommand(Commands.TconResolution);
        SendData(0x02);
        SendData(0x58);
        SendData(0x01);
        SendData(0xC0);
        SendCommand(Commands.FlashMode);
        SendData(0xAA);
    }

    /// <summary>
    /// Turn display on after sleep
    /// </summary>
    protected override void TurnDisplayOn()
    {
        SendCommand(Commands.PowerOn);
        WaitUntilReady();
        SendCommand(Commands.DisplayRefresh);
        Thread.Sleep(100);
        WaitUntilReady();
    }

    /// <summary>
    /// Convert color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    protected override byte ToByte(Color color)
    {
        if (color.Monochrome)
            return (byte)(color.Red <= 85 ? HardwareColors.Black : HardwareColors.White);
        else if (color == Colors.Black)
            return (byte)HardwareColors.Black;
        else if (color == Colors.White)
            return (byte)HardwareColors.White;
        else if (color == Colors.Green)
            return (byte)HardwareColors.Green;
        else if (color == Colors.Blue)
            return (byte)HardwareColors.Blue;
        else if (color == Colors.Red)
            return (byte)HardwareColors.Red;
        else if (color == Colors.Yellow)
            return (byte)HardwareColors.Yellow;
        else if (color == Colors.Orange)
            return (byte)HardwareColors.Orange;
        else
            return (byte)HardwareColors.Clean;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Send command
    /// </summary>
    /// <param name="command">Command to send</param>
    private void SendCommand(Commands command) => SendCommand((byte)command);

    /// <summary>
    /// Fill screen
    /// </summary>
    /// <param name="color"></param>
    private void FillColor(Color color)
    {
        var outputLine = CreateScanLine(color);
        SendCommand(StartDataTransmissionCommand);
        for (var y = 0; y < Height; y++)
            SendData(outputLine);
        SendCommand(StopDataTransmissionCommand);
    }
    #endregion

}