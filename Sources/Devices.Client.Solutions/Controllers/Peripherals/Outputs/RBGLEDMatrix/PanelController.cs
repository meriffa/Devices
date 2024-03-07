using Color = Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Color;
using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;
using SixLabors.ImageSharp;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Panel controller
/// </summary>
[Verb("RBGLEDMatrix-Panel", HelpText = "RGB LED Matrix Panel operation.")]
public class PanelController : RBGLEDMatrixController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Panel operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Panel operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display canvas
    /// </summary>
    /// <param name="matrix"></param>
    private static void DisplayCanvas(Matrix matrix)
    {
        var canvas = matrix.CreateOffscreenCanvas();
        // Positions
        var width = canvas.Width - 1;
        var height = canvas.Height - 1;
        var centerX = canvas.Width / 2;
        var centerY = canvas.Height / 2;
        // Font
        using var font = GetFont(Path.Combine(AppContext.BaseDirectory, "Font.bdf"));
        var fontX = centerX - "HH:mm:ss".Length * 6 / 2;
        var fontY = font.Baseline + 1;
        // Image
        Configuration.Default.PreferContiguousImageBuffers = true;
        using var image = Image.Load<SixLabors.ImageSharp.PixelFormats.Rgb24>(Path.Combine(AppContext.BaseDirectory, "Image.png"));
        if (!image.DangerousTryGetSinglePixelMemory(out var imageMemory))
            throw new("Pixel buffer read failed.");
        var imageData = MemoryMarshal.Cast<SixLabors.ImageSharp.PixelFormats.Rgb24, Color>(imageMemory.Span);
        // Render
        while (IsRunning())
        {
            canvas.Fill(Color.DarkGray);
            canvas.DrawLine(0, 0, width - 1, 0, Color.Red);
            canvas.DrawLine(width, 0, width, height - 1, Color.Green);
            canvas.DrawLine(1, height, width, height, Color.Blue);
            canvas.DrawLine(0, 1, 0, height, Color.White);
            canvas.SetPixel(centerX, centerY, Color.Yellow);
            canvas.DrawCircle(centerX, centerY, 5, Color.Purple);
            canvas.DrawText(font, fontX, fontY, Color.Cyan, DateTime.Now.ToString("HH:mm:ss"));
            canvas.SetPixels(centerX - image.Width / 2, height - image.Height - 1, image.Width, image.Height, imageData);
            matrix.SwapOnVSync(canvas);
            Thread.Sleep(1000);
        }
    }
    #endregion

}