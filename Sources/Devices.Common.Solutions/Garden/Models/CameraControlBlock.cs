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
    public bool FocusRangeRequest;
    [FieldOffset(8)]
    public int FocusMinimum;
    [FieldOffset(12)]
    public int FocusMaximum;
    [FieldOffset(16)]
    public bool FocusRequest;
    [FieldOffset(20)]
    public int FocusValue;
    [FieldOffset(24)]
    public bool ZoomRequest;
    [FieldOffset(28)]
    public int ZoomValue;
}