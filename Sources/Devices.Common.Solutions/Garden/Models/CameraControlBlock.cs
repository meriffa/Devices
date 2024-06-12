using System.Runtime.InteropServices;

namespace Devices.Common.Solutions.Garden.Models;

/// <summary>
/// Camera control block
/// </summary>
[StructLayout(LayoutKind.Explicit, Pack = 1, CharSet = CharSet.Unicode)]
public struct CameraControlBlock
{
    [FieldOffset(0)]
    public bool StopRequest;
    [FieldOffset(4)]
    public bool ZoomRequest;
    [FieldOffset(8)]
    public int ZoomValue;
    [FieldOffset(12)]
    public bool FocusRequest;
    [FieldOffset(16)]
    public int FocusValue;
}