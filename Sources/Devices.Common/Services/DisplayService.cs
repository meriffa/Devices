using Microsoft.Extensions.Logging;

namespace Devices.Common.Services;

/// <summary>
/// Display service
/// </summary>
/// <param name="logger"></param>
public class DisplayService(ILogger<DisplayService>? logger)
{

    #region Constants
    private const string DATE_FORMAT = "yyyy-MM-dd HH:mm:ss.ffff";
    #endregion

    #region Private Fields
    private readonly ILogger<DisplayService>? logger = logger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Display new line
    /// </summary>
    public static void WriteText() => Console.WriteLine();

    /// <summary>
    /// Display text
    /// </summary>
    /// <param name="value"></param>
    public static void WriteText(string value) => Console.WriteLine(value);

    /// <summary>
    /// Display information
    /// </summary>
    /// <param name="value"></param>
    /// <param name="logValue"></param>
    public void WriteInformation(string value, bool logValue = true)
    {
        Console.WriteLine($"[{DateTime.Now.ToString(DATE_FORMAT)}]: {value}");
        if (logValue)
            logger?.LogInformation("{Text}", value);
    }

    /// <summary>
    /// Display warning
    /// </summary>
    /// <param name="value"></param>
    public void WriteWarning(string value)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        WriteInformation($"[WARNING] {value}", false);
        Console.ForegroundColor = color;
        logger?.LogWarning("{Text}", value);
    }

    /// <summary>
    /// Display error
    /// </summary>
    /// <param name="exception"></param>
    public void WriteError(Exception exception)
    {
        WriteError($"[ERROR] {exception.Message}", false);
        logger?.LogError(exception, "{Error}", exception.Message);
    }

    /// <summary>
    /// Display error
    /// </summary>
    /// <param name="value"></param>
    /// <param name="logValue"></param>
    public void WriteError(string value, bool logValue = true)
    {
        var color = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        WriteInformation(value, false);
        Console.ForegroundColor = color;
        if (logValue)
            logger?.LogError("{Error}", value);
    }
    #endregion

}