using System.Runtime.InteropServices;
using System.Text.Json;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// Device matrix options
/// </summary>
/// <param name="options"></param>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct MatrixOptionsDevice(MatrixOptions options)
{

    #region Public Fields
    public IntPtr hardware_mapping = Marshal.StringToHGlobalAnsi(JsonSerializer.Serialize(options.HardwareMapping)[1..^1]);
    public int rows = options.Rows;
    public int cols = options.Columns;
    public int chain_length = options.Chains;
    public int parallel = options.ParallelChains;
    public int pwm_bits = options.PWMBits;
    public int pwm_lsb_nanoseconds = options.PWMLSBNanoseconds;
    public int pwm_dither_bits = options.PWMDitherBits;
    public int brightness = options.Brightness;
    public int scan_mode = (int)options.ScanMode;
    public int row_address_type = (int)options.RowAddressType;
    public int multiplexing = (int)options.Multiplexing;
    public byte disable_hardware_pulsing = (byte)(options.DisableHardwarePulsing ? 1 : 0);
    public byte show_refresh_rate = (byte)(options.ShowRefreshRate ? 1 : 0);
    public byte inverse_colors = (byte)(options.InverseColors ? 1 : 0);
    public IntPtr led_rgb_sequence = Marshal.StringToHGlobalAnsi(options.RGBSequence);
    public IntPtr pixel_mapper_config = Marshal.StringToHGlobalAnsi(options.PixelMapper);
    public IntPtr panel_type = Marshal.StringToHGlobalAnsi(options.PanelType);
    public int limit_refresh_rate_hz = options.LimitRefreshRateHz;
    #endregion

}