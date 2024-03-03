namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Matrix options
/// </summary>
public struct MatrixOptions
{

    #region Public Fields
    /// <summary>
    /// Name of the hardware mapping used.
    /// </summary>
    public HardwareMappingType HardwareMapping = HardwareMappingType.AdafruitHATPWM;

    /// <summary>
    /// Number of rows supported by the display.
    /// </summary>
    public int Rows = 64;

    /// <summary>
    /// Number of columns per panel.
    /// </summary>
    public int Columns = 64;

    /// <summary>
    /// Number of displays daisy-chained together.
    /// </summary>
    public int Chains = 1;

    /// <summary>
    /// Number of parallel chains.
    /// </summary>
    public int ParallelChains = 1;

    /// <summary>
    /// Type of multiplexing.
    /// </summary>
    public MultiplexingType Multiplexing = MultiplexingType.Direct;

    /// <summary>
    /// Set PWM bits used for output (1-11).
    /// </summary>
    public int PWMBits = 11;

    /// <summary>
    /// Change the base time-unit for the on-time in the lowest significant bit in nanoseconds. Higher numbers provide better quality (more accurate color, less ghosting), but have a negative impact on the frame rate.
    /// </summary>
    public int PWMLSBNanoseconds = 130;

    /// <summary>
    /// The lower bits can be time-dithered for higher refresh rate.
    /// </summary>
    public int PWMDitherBits = 0;

    /// <summary>
    /// The initial brightness of the panel in percent.
    /// </summary>
    public int Brightness = 100;

    /// <summary>
    /// Scan mode.
    /// </summary>
    public ScanModeType ScanMode = ScanModeType.Progressive;

    /// <summary>
    /// Default row address type is 0, corresponding to direct setting of the row, while row address type 1 is used for panels that only have A/B, typically some 64x64 panels.
    /// </summary>
    public RowAddressType RowAddressType = RowAddressType.Default;

    /// <summary>
    /// In case the internal sequence of mapping is not <c>"RGB"</c>, this contains the real mapping. Some panels mix up these colors.
    /// </summary>
    public string? RGBSequence;

    /// <summary>
    /// Semicolon-separated list of pixel-mappers to arrange pixels. Available: "", "Mirror", "Rotate", "U-mapper", "V-mapper". Optional params after a colon e.g. "U-mapper;Rotate:90".
    /// </summary>
    public string? PixelMapper;

    /// <summary>
    /// Panel type. Typically just empty, but certain panels (FM6126) require an initialization sequence.
    /// </summary>
    public string? PanelType;

    /// <summary>
    /// Allow to use the hardware subsystem to create pulses. This won't do anything if output enable is not connected to GPIO 18.
    /// </summary>
    public bool DisableHardwarePulsing = false;

    /// <summary>
    /// Display refresh rate flag.
    /// </summary>
    public bool ShowRefreshRate = false;

    /// <summary>
    /// Inverse colors flag.
    /// </summary>
    public bool InverseColors = false;

    /// <summary>
    /// Limit refresh rate of LED panel. This will help on a loaded system to keep a constant refresh rate. 0 for no limit.
    /// </summary>
    public int LimitRefreshRateHz = 0;

    /// <summary>
    /// Slowdown GPIO. Needed for faster Pis/slower panels.
    /// </summary>
    public int GPIOSlowdown = 1;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public MatrixOptions() { }
    #endregion

}