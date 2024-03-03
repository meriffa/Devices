using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Devices.Common.Extensions;

/// <summary>
/// Custom JsonStringEnumConverter
/// </summary>
/// <typeparam name="T"></typeparam>
public class JsonStringEnumConverterEx<T> : JsonConverter<T> where T : struct, Enum
{

    #region Private Fields
    private readonly Dictionary<T, string> valuesToString = [];
    private readonly Dictionary<string, T> stringToValues = [];
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    public JsonStringEnumConverterEx()
    {
        var type = typeof(T);
        foreach (var value in Enum.GetValues<T>())
        {
            stringToValues.Add(value.ToString(), value);
            var attribute = type.GetMember(value.ToString()).FirstOrDefault()?.GetCustomAttributes(typeof(EnumMemberAttribute), false).Cast<EnumMemberAttribute>().FirstOrDefault();
            if (attribute?.Value != null)
            {
                valuesToString.Add(value, attribute.Value);
                stringToValues.TryAdd(attribute.Value, value);
            }
            else
                valuesToString.Add(value, value.ToString());
        }
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Reads and converts the JSON to type T
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var value = reader.GetString();
        return value != null && stringToValues.TryGetValue(value, out var enumValue) ? enumValue : default;
    }

    /// <summary>
    /// Writes a specified value as JSON
    /// </summary>
    /// <param name="writer"></param>
    /// <param name="value"></param>
    /// <param name="options"></param>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(valuesToString[value]);
    }
    #endregion

}