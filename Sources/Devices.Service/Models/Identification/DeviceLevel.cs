using System.Text.Json.Serialization;

namespace Devices.Service.Models.Identification;

/// <summary>
/// Device level
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeviceLevel
{
    Green,
    Amber,
    Red
}