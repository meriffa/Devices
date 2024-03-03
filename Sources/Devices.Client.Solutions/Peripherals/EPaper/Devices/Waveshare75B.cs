using Devices.Client.Solutions.Peripherals.EPaper.Common;
using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;

namespace Devices.Client.Solutions.Peripherals.EPaper.Devices;

/// <summary>
/// Waveshare 7.5inch e-Paper (B) (800x480, Black, White, Red)
/// </summary>
public sealed class Waveshare75B : DisplayBase
{

    #region Colors
    /// <summary>
    /// Hardware colors
    /// </summary>
    private static class HardwareColors
    {
        public const byte White = 0x01;
        public const byte Black = 0x00;
        public const byte Red = 0x01;
        public const byte NonRed = 0x00;
    }
    #endregion

    #region Commands
    /// <summary>
    /// Hardware commands
    /// </summary>
    internal enum Commands
    {
        None = -1, // Default value
        PanelSetting = 0x00, // Panel Setting (PSR) (R00H)
        PowerSetting = 0x01, // Power Setting (PWR) (R01H)
        PowerOff = 0x02, // Power OFF (POF) (R02H)
        PowerOffSequenceSetting = 0x03, // Power OFF Sequence Setting (PFS) (R03H)
        PowerOn = 0x04, // Power ON (PON) (R04H)
        PowerOnMeasure = 0x05, // Power ON Measure (PMES) (R05H)
        BoosterSoftStart = 0x06, // Booster Soft Start (BTST) (R06H)
        DeepSleep = 0x07, // Deep sleep (DSLP) (R07H)
        DataStartTransmission1 = 0x10, // Data Start Transmission 1 (DTM1, White/Black) (R10H)
        DataStop = 0x11, // Data stop (DSP) (R11H)
        DisplayRefresh = 0x12, // Display Refresh (DRF) (R12H)
        DataStartTransmission2 = 0x13, // Display Start transmission 2 (DTM2, Red) (R13H)
        DualSpi = 0x15, // Dual SPI (R15H)
        AutoSequence = 0x17, // Auto Sequence (AUTO) (R17H)
        LutOption = 0x2B, // KW LUT option (KWOPT) (R2BH)
        PllControl = 0x30, // PLL Control (PLL) (R30H)
        TemperatureSensorCommand = 0x40, // Temperature Sensor Calibration (TSC) (R40H)
        TemperatureCalibration = 0x41, // Temperature Sensor Internal/External (TSE) (R41H)
        TemperatureSensorWrite = 0x42, // Temperature Sensor Write (TSW) (R42H)
        TemperatureSensorRead = 0x43, // Temperature Sensor Read (TSR) (R43H)
        PanelBreakCheck = 0x44, // Panel Break Check (PBC) (R44H)
        VcomAndDataIntervalSetting = 0x50, // VCOM and Data Interval Setting (CDI) (R50H)
        LowPowerDetection = 0x51, // Low Power Detection (LPD) (R51H)
        EndVoltageSetting = 0x52, // End Voltage Setting (EVS) (R52H)
        TconSetting = 0x60, // TCON Setting (TCON) (R60h)
        TconResolution = 0x61, // Resolution Setting (TRES) (R61H)
        GateSourceStartSetting = 0x65, // Gate/Source Start setting (GSST) (R65H)
        Revision = 0x70, // Revision (REV) (R70H)
        GetStatus = 0x71, // Get status (FLG) (R71H)
        AutoMeasurementVcom = 0x80, // Auto measure VCOM (AMV) (R80H)
        ReadVcomValue = 0x81, // VCOM Value (VV) (R81H)
        VcmDcSetting = 0x82, // VCOM-DC Setting (VDCS) (R82H)
        PartialWindow = 0x90, // Partial Window (PTL) (R90H)
        PartialIn = 0x91, // Partial In (PTIN) (R91H)
        PartialOut = 0x92, // Partial Out (PTOUT) (R92H)
        ProgramMode = 0xA0, // Program Mode (PGM) (RA0H)
        ActiveProgramming = 0xA1, // Active Programming (APG) (RA1H)
        ReadOtp = 0xA2, // Read OTP (ROTP) (RA2H)
        CascadeSetting = 0xE0, // Cascade Setting (CCSET) (RE0H)
        PowerSaving = 0xE3, // Power Saving (PWS) (RE3H)
        LvdVoltageSelect = 0xE4, // LVD Voltage Select (LVSEL) (RE4H)
        ForceTemperature = 0xE5, // Force Temperature (TSSET) (RE5H)
        TemperatureBoundaryPhaseC2 = 0xE7 // Temperature Boundary Phase-C2 (TSBDRY) (RE7H)
    }
    #endregion

    #region Properties
    /// <summary>
    /// Display width
    /// </summary>
    public override int Width { get; } = 800;

    /// <summary>
    /// Display height
    /// </summary>
    public override int Height { get; } = 480;

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override Color[] SupportedColors { get; } = [Colors.White, Colors.Black, Colors.Red];

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override byte[] DeviceColors { get; } = [HardwareColors.White, HardwareColors.Black, HardwareColors.Red];

    /// <summary>
    /// Device pixels per byte
    /// </summary>
    public override int PixelsPerByte { get; } = 8;

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
    protected override byte StopDataTransmissionCommand { get; } = byte.MaxValue;

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
        FillColor(Commands.DataStartTransmission1, Colors.White);
        FillColor(Commands.DataStartTransmission2, Colors.Black);
        TurnDisplayOn();
    }

    /// <summary>
    /// Clear display to black
    /// </summary>
    public override void ClearBlack()
    {
        FillColor(Commands.DataStartTransmission1, Colors.Black);
        FillColor(Commands.DataStartTransmission2, Colors.Black);
        TurnDisplayOn();
    }

    /// <summary>
    /// Power controller on (do not use with sleep mode)
    /// </summary>
    public override void PowerOn()
    {
        SendCommand(Commands.PowerOn);
        DeviceWaitUntilReady();
    }

    /// <summary>
    /// Power controller off (do not use with sleep mode)
    /// </summary>
    public override void PowerOff()
    {
        SendCommand(Commands.PowerOff);
        DeviceWaitUntilReady();
    }

    /// <summary>
    /// Wait until display ready
    /// </summary>
    public void DeviceWaitUntilReady()
    {
        WaitUntilReady();
        Thread.Sleep(200);
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Initialize device
    /// </summary>
    protected override void InitializeDevice()
    {
        Reset();
        SendCommand(Commands.PowerSetting);
        SendData(0x07); // VGH: 20V
        SendData(0x07); // VGL: -20V
        SendData(0X3F); // VDH: 15V
        SendData(0x3F); // VDL: -15V
        SendCommand(Commands.PowerOn);
        Thread.Sleep(100);
        DeviceWaitUntilReady();
        SendCommand(Commands.PanelSetting);
        SendData(0x0F); // KW-3f   KWR-2F	BWROTP 0f	BWOTP 1f
        SendCommand(Commands.TconResolution);
        SendData(0x03); // source 800
        SendData(0x20);
        SendData(0x01); // gate 480
        SendData(0xE0);
        SendCommand(Commands.DualSpi);
        SendData(0x00);
        SendCommand(Commands.VcomAndDataIntervalSetting);
        SendData(0x11);
        SendData(0x07);
        SendCommand(Commands.TconSetting);
        SendData(0x22);
        SendCommand(Commands.GateSourceStartSetting);  // Resolution setting
        SendData(0x00);
        SendData(0x00); // 800*480
        SendData(0x00);
        SendData(0x00);
    }

    /// <summary>
    /// Turn display on after sleep
    /// </summary>
    protected override void TurnDisplayOn()
    {
        SendCommand(Commands.DisplayRefresh);
        Thread.Sleep(100);
        DeviceWaitUntilReady();
    }

    /// <summary>
    /// Convert color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    protected override byte ToByte(Color color)
    {
        if (!color.Monochrome)
            color.Desaturate();
        return color.Red >= COLOR_DISPLAY_THRESHOLD ? HardwareColors.Red : HardwareColors.NonRed;
    }

    /// <summary>
    /// Return display writer
    /// </summary>
    /// <returns></returns>
    protected override IDisplayWriter GetDisplayWriter() => new Waveshare75BWriter(this);
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
    /// <param name="command"></param>
    /// <param name="color"></param>
    private void FillColor(Commands command, Color color)
    {
        var outputLine = CreateScanLine(color);
        SendCommand(command);
        for (var y = 0; y < Height; y++)
            SendData(outputLine);
    }
    #endregion

}