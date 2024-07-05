namespace Devices.Common.Models;

/// <summary>
///  Task type
/// </summary>
[Flags]
public enum TaskType
{
    None = 0,
    Identity = 1,
    Monitoring = 2,
    Configuration = 4
}