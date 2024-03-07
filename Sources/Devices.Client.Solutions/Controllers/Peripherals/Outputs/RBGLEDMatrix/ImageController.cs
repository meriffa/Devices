using Color = Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Color;
using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Image controller
/// </summary>
[Verb("RBGLEDMatrix-Image", HelpText = "LED matrix image operation.")]
public class ImageController : RBGLEDMatrixController
{

    #region Properties
    /// <summary>
    /// Image file name
    /// </summary>
    [Option('f', "file", Required = true, HelpText = "RGB LED Matrix image file name.")]
    public string FileName { get; set; } = null!;

    /// <summary>
    /// LED matrix speed
    /// </summary>
    [Option('s', "speed", Required = false, Default = 0, HelpText = "RGB LED Matrix image speed.")]
    public int Speed { get; set; }

    /// <summary>
    /// LED matrix speed
    /// </summary>
    [Option('d', "direction", Required = false, Default = HorizontalDirection.Left, HelpText = "RGB LED Matrix image direction.")]
    public HorizontalDirection Direction { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Image operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Image operation completed.");
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
        if (!File.Exists(FileName))
            throw new Exception($"Image file '{FileName}' not found.");
        Configuration.Default.PreferContiguousImageBuffers = true;
        using var image = Image.Load<Rgb24>(FileName);
        image.Mutate(o => o.Resize(canvas.Width, canvas.Height));
        var frameIndex = -1;
        var frames = image.Frames.Select(f =>
        (
            Pixels: f.DangerousTryGetSinglePixelMemory(out var memory) ? memory : throw new("Pixel buffer read failed."),
            Delay: f.Metadata.GetGifMetadata().FrameDelay * 10
        )).ToArray();
        var width = canvas.Width - 1;
        var x = Speed > 0 ? (Direction == HorizontalDirection.Left ? width : -width) : 0;
        while (IsRunning())
        {
            frameIndex = (frameIndex + 1) % frames.Length;
            canvas.SetPixels(x, 0, canvas.Width, canvas.Height, MemoryMarshal.Cast<Rgb24, Color>(frames[frameIndex].Pixels.Span));
            x = GetPositionX(width, x);
            matrix.SwapOnVSync(canvas);
            Thread.Sleep(frames[frameIndex].Delay);
        }
    }

    /// <summary>
    /// Return X position
    /// </summary>
    /// <param name="width"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    private int GetPositionX(int width, int x)
    {
        if (Speed > 0)
            if (Direction == HorizontalDirection.Left)
            {
                if ((x -= Speed) + width < 0)
                    x = width;
            }
            else if (Direction == HorizontalDirection.Right)
            {
                if ((x += Speed) > width)
                    x = -width;
            }
        return x;
    }
    #endregion

}