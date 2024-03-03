using Color = Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Color;
using CommandLine;
using Devices.Client.Solutions.Peripherals.FFmpeg;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Video controller
/// </summary>
[Verb("RBGLEDMatrix-Video", HelpText = "RGB LED Matrix Video operation.")]
public class VideoController : RBGLEDMatrixController
{

    #region Properties
    /// <summary>
    /// Video file name
    /// </summary>
    [Option('f', "file", Required = true, HelpText = "RGB LED Matrix video file name.")]
    public string FileName { get; set; } = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"RGB LED Matrix Video operation started.");
        using var matrix = GetMatrix();
        var canvas = matrix.CreateOffscreenCanvas();
        if (!File.Exists(FileName))
            throw new Exception($"Video file '{FileName}' not found.");
        Configuration.Default.PreferContiguousImageBuffers = true;
        var player = new FFmpegPlayer(FileName, canvas.Width, canvas.Height, true);
        player.OnRenderFrame += (frame) => { DisplayCanvas(matrix, canvas, frame); };
        player.Start();
        while (IsRunning())
            Thread.Yield();
        player.Stop();
        DisplayService.WriteInformation($"RGB LED Matrix Video operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display canvas
    /// </summary>
    /// <param name="matrix"></param>
    /// <param name="canvas"></param>
    /// <param name="frame"></param>
    private static void DisplayCanvas(Matrix matrix, Canvas canvas, ReadOnlySpan<byte> frame)
    {
        using var image = Image.Load<Rgb24>(frame.ToArray());
        if (image.Width != canvas.Width || image.Height != canvas.Height)
            image.Mutate(o => o.Resize(canvas.Width, canvas.Height));
        if (!image.DangerousTryGetSinglePixelMemory(out var memory))
            throw new("Pixel buffer read failed.");
        canvas.SetPixels(0, 0, canvas.Width, canvas.Height, MemoryMarshal.Cast<Rgb24, Color>(memory.Span));
        matrix.SwapOnVSync(canvas);
    }
    #endregion

}