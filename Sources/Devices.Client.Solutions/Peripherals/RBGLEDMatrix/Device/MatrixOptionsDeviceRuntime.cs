using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// Runtime device matrix options
/// </summary>
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct MatrixOptionsDeviceRuntime()
{

    #region Public Fields
    public int gpio_slowdown = 0;   // --led-slowdown-gpio (0-4)
    public int daemon = 0;          // --led-daemon (-1 = Disabled, 0 = Off, 1 = On)
    public int drop_privileges = 0; // --led-drop-privs (-1 = Disabled, 0 = Off, 1 = On)
    public byte do_gpio_init = 1;   // Enable / disable GPIO initialization
    public IntPtr drop_priv_user;   // Default = "daemon"
    public IntPtr drop_priv_group;  // Default = "daemon"
    #endregion

}