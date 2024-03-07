using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Text controller
/// </summary>
[Verb("RBGLEDMatrix-Text", HelpText = "RGB LED Matrix Text operation.")]
public class TextController : RBGLEDMatrixController
{

    #region Properties
    /// <summary>
    /// LED matrix text
    /// </summary>
    [Option('t', "text", Required = true, HelpText = "RGB LED Matrix text.")]
    public string Text { get; set; } = null!;

    /// <summary>
    /// LED matrix speed
    /// </summary>
    [Option('s', "speed", Required = false, Default = 5, HelpText = "RGB LED Matrix text speed (0-100).")]
    public int Speed { get; set; }

    /// <summary>
    /// LED matrix foreground color
    /// </summary>
    [Option('o', "foregroundColor", Required = false, Default = "255,255,255", HelpText = "RGB LED Matrix text foreground color (R,G,B).")]
    public string ForegroundColor { get; set; } = null!;

    /// <summary>
    /// LED matrix background color
    /// </summary>
    [Option('g', "backgroundColor", Required = false, Default = "0,0,0", HelpText = "RGB LED Matrix text background color (R,G,B).")]
    public string BackgroundColor { get; set; } = null!;

    /// <summary>
    /// LED matrix foreground color
    /// </summary>
    [Option('f', "font", Required = false, Default = "", HelpText = "RGB LED Matrix text font.")]
    public string Font { get; set; } = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Text operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Text operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display canvas
    /// </summary>
    /// <param name="matrix"></param>
    private void DisplayCanvas(Matrix matrix)
    {
        var canvas = matrix.CreateOffscreenCanvas();
        using var font = GetFont(GetFontPath(Font));
        var width = canvas.Width - 1;
        var x = Speed > 0 ? width : 0;
        var y = canvas.Height / 2 + font.Baseline - font.Height / 2;
        var delay = Speed > 0 ? 100 / Speed : 0;
        var foregroundColor = GetColor(ForegroundColor);
        var backgroundColor = GetColor(BackgroundColor);
        while (IsRunning())
        {
            canvas.Fill(backgroundColor);
            var length = canvas.DrawText(font, x, y, foregroundColor, Text);
            if (Speed > 0 && --x + length < 0)
                x = width;
            matrix.SwapOnVSync(canvas);
            Thread.Sleep(delay);
        }
    }

    /// <summary>
    /// Return color
    /// </summary>
    /// <param name="color"></param>
    /// <returns></returns>
    private static Color GetColor(string color)
    {
        var values = color.Split(',');
        return new(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]));
    }

    /// <summary>
    /// Return font path
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string GetFontPath(string path)
    {
        var result = string.IsNullOrEmpty(path) ? Path.Combine(AppContext.BaseDirectory, "Font.bdf") : path;
        if (!File.Exists(result))
            throw new($"Font '{result}' not found.");
        return result;
    }
    #endregion

}