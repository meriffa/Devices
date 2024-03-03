using Devices.Client.Solutions.Peripherals.EPaper.Common;

namespace Devices.Client.Solutions.Peripherals.EPaper.Devices;

/// <summary>
/// Waveshare 7.5inch e-Paper (800x480, Black, White)
/// </summary>
public sealed class Waveshare75 : DisplayBase
{

    #region Colors
    /// <summary>
    /// Hardware colors
    /// </summary>
    private enum HardwareColors
    {
        Black = 0x01,
        White = 0x00
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
        PowerOnMeasure = 0x05, // Power ON Measure (PMES) (R05H)
        BoosterSoftStart = 0x06, // Booster Soft Start (BTST) (R06H)
        DeepSleep = 0x07, // Deep Sleep (DSLP) (R07H)
        DataStartTransmission1 = 0x10, // Data Start Transmission 1 (DTM1) (R10H)
        DataStop = 0x11, // Data Stop Transmission (DSP) (R11H)
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
        LowPowerDetection = 0x51, // Low Power Detection (LPD) (R51h)
        EndVoltageSetting = 0x52, // End Voltage Setting (EVS) (R52H)
        TconSetting = 0x60, // TCON Setting (TCON) (R60h) Undocumented
        TconResolution = 0x61, // Resolution Setting (TRES) (R61H)
        GateSourceStartSetting = 0x65, // Gate/Source Start setting (GSST) (R65H)
        Revision = 0x70, // Revision (REV) (R70H)
        GetStatus = 0x71, // Get Status (FLG) (R71H)
        AutoMeasurementVcom = 0x80, // Auto Measure VCOM (AMV) (R80h)
        ReadVcomValue = 0x81, // VCOM Value (VV) (R81h)
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
    public override Color[] SupportedColors { get; } = [Colors.White, Colors.Black];

    /// <summary>
    /// Supported device colors
    /// </summary>
    public override byte[] DeviceColors { get; } = [(byte)HardwareColors.White, (byte)HardwareColors.Black];

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
    protected override byte StartDataTransmissionCommand { get; } = (byte)Commands.DataStartTransmission2;

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
        FillColor(Commands.DataStartTransmission2, Colors.White);
        TurnDisplayOn();
    }

    /// <summary>
    /// Clear display to black
    /// </summary>
    public override void ClearBlack()
    {
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
        SendCommand(Commands.BoosterSoftStart);
        SendData(0x17);
        SendData(0x17);
        SendData(0x27);
        SendData(0x17);
        SendCommand(Commands.PowerSetting);
        SendData(0x07); // VGH: 20V
        SendData(0x17); // VGL: -20V
        SendData(0x3f); // VDH: 15V
        SendData(0x3f); // VDL: -15V
        SendCommand(Commands.PowerOn);
        Thread.Sleep(100);
        DeviceWaitUntilReady();
        SendCommand(Commands.PanelSetting);
        SendData(0x1F); // KW-3f   KWR-2F	BWROTP 0f	BWOTP 1f
        SendCommand(Commands.TconResolution);
        SendData(0x03); // source 800
        SendData(0x20);
        SendData(0x01); // gate 480
        SendData(0xe0);
        SendCommand(Commands.DualSpi);
        SendData(0x00);
        SendCommand(Commands.VcomAndDataIntervalSetting);
        SendData(0x10);
        SendData(0x07);
        SendCommand(Commands.TconSetting);
        SendData(0x22);
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
    protected override byte ToByte(Color color) => (byte)(color.Red < COLOR_DISPLAY_THRESHOLD ? HardwareColors.Black : HardwareColors.White);
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