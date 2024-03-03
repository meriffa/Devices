namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Row address type
/// </summary>
public enum RowAddressType : int
{
    Default = 0,
    ABAddressed = 1,
    DirectRowSelect = 2,
    ABCAddressed = 3,
    ABCShiftAndDEDirect = 4
}