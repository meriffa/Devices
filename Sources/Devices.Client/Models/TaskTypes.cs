namespace Devices.Client.Models;

/// <summary>
///  Task types
/// </summary>
[Flags]
public enum TaskTypes
{
    None = 0,
    Identity = 1,
    Monitoring = 2,
    Configuration = 4
}