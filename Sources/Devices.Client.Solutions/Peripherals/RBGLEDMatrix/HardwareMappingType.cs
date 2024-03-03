using Devices.Common.Extensions;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix;

/// <summary>
/// Hardware mapping type
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverterEx<HardwareMappingType>))]
public enum HardwareMappingType
{
    [EnumMember(Value = "regular")]
    Regular,
    [EnumMember(Value = "adafruit-hat")]
    AdafruitHAT,
    [EnumMember(Value = "adafruit-hat-pwm")]
    AdafruitHATPWM,
    [EnumMember(Value = "regular-pi1")]
    RegularPi1,
    [EnumMember(Value = "classic")]
    Classic,
    [EnumMember(Value = "classic-pi1")]
    ClassicPi1
}