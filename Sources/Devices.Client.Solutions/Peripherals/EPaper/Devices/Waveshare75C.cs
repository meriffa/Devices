using Devices.Client.Solutions.Peripherals.EPaper.Common;

namespace Devices.Client.Solutions.Peripherals.EPaper.Devices;

/// <summary>
/// WaveShare 7.5inch e-Paper (C) (640x384, Black, White, Yellow)
/// </summary>
public sealed class Waveshare75C : DisplayBase
{

    #region Colors
    /// <summary>
    /// Hardware colors
    /// </summary>
    private enum HardwareColors
    {
        Black = 0x00,
        Gray = 0x02,
        White = 0x03,
        Yellow = 0x04
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
        LutForVcom = 0x20, // VCOM LUT (LUTC) (R20H)
        LutBlue = 0x21, // Black LUT (LUTB) (R21H)
        LutWhite = 0x22, // White LUT(LUTW) (R22H)
        LutGray1 = 0x23, // Gray1 LUT (LUTG1) (R23H)
        LutGray2 = 0x24, // Gray2 LUT (LUTG2) (R24H)
        LutRed0 = 0x25, // Red0 LUT (LUTR0) (R25H)
        LutRed1 = 0x26, // Red1 LUT (LUTR1) (R26H)
        LutRed2 = 0x27, // Red2 LUT (LUTR2) (R27H)
        LutRed3 = 0x28, // Red3 LUT (LUTR3) (R28H)
        LutXon = 0x29, // XON LUT (LUTXON) (R29H)
        PllControl = 0x30, // PLL Control (PLL) (R30H)
        TemperatureSensorCommand = 0x40, // Temperature Sensor Calibration (TSC) (R40H)
        TemperatureCalibration = 0x41, // Temperature Sensor Internal/External (TSE) (R41H)
        TemperatureSensorWrite = 0x42, // Temperature Sensor Write (TSW) (R42H)
        TemperatureSensorRead = 0x43, // Temperature Sensor Read (TSR) (R43H)
        VcomAndDataIntervalSetting = 0x50, // VCOM and Data Interval Setting (CDI) (R50H)
        LowPowerDetection = 0x51, // Low Power Detection (LPD) (R51h)
        TconSetting = 0x60, // TCON Setting (TCON) (R60h) Undocumented
        TconResolution = 0x61, // Resolution Setting (TRES) (R61H)
        SpiFlashControl = 0x65, // SPI Flash Control (DAM) (R65H)
        Revision = 0x70, // Revision (REV) (R70H)
        GetStatus = 0x71, // Get Status (FLG) (R71H)
        AutoMeasurementVcom = 0x80, // Auto Measure VCOM (AMV) (R80h)
        ReadVcomValue = 0x81, // VCOM Value (VV) (R81h)
        VcmDcSetting = 0x82, // VCOM-DC Setting (VDCS) (R82H)
        FlashMode = 0xE5 // Flash Mode (RE5H) Undocumented
    }
    #endregion

    #region Properties
    /// <summary>
    /// Display width
    /// </summary>
    public override int Width { get; } = 640;

    /// <summary>
    /// Display height
    /// </summary>
    public override int Height { get; } = 384;

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override Color[] SupportedColors { get; } = [Colors.Black, Colors.Gray, Colors.White, Colors.Yellow];

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override byte[] DeviceColors { get; } = [(byte)HardwareColors.Black, (byte)HardwareColors.Gray, (byte)HardwareColors.White, (byte)HardwareColors.Yellow];

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
        SendCommand(Commands.PowerSetting); // POWER_SETTING
        SendData(0x37);
        SendData(0x00);
        SendCommand(Commands.PanelSetting);
        SendData(0xCF);
        SendData(0x08);
        SendCommand(Commands.PllControl);
        SendData(0x3A); // PLL:  0-15:0x3C, 15+:0x3A
        SendCommand(Commands.VcmDcSetting);
        SendData(0x28); // all temperature  range
        SendCommand(Commands.BoosterSoftStart);
        SendData(0xc7);
        SendData(0xcc);
        SendData(0x15);
        SendCommand(Commands.VcomAndDataIntervalSetting);
        SendData(0x77);
        SendCommand(Commands.TconSetting);
        SendData(0x22);
        SendCommand(Commands.SpiFlashControl);
        SendData(0x00);
        SendCommand(Commands.TconResolution);
        SendData((byte)(Width >> 8)); // source 640
        SendData((byte)(Width & 0xff));
        SendData((byte)(Height >> 8)); // gate 384
        SendData((byte)(Height & 0xff));
        SendCommand(Commands.FlashMode);
        SendData(0x03);
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
        {
            if (color.Red <= 85)
                return (byte)HardwareColors.Black;
            else if (color.Red <= 170)
                return (byte)HardwareColors.Gray;
            else
                return (byte)HardwareColors.White;
        }
        return (byte)(color.Red >= 64 ? HardwareColors.Yellow : HardwareColors.Black);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Send command
    /// </summary>
    /// <param name="command"></param>
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