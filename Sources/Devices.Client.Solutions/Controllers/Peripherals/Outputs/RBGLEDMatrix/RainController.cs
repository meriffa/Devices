using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Rain controller
/// </summary>
[Verb("RBGLEDMatrix-Rain", HelpText = "LED matrix matrix rain operation.")]
public class RainController : RBGLEDMatrixController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Rain operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Rain operation completed.");
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
        int maxHeight = 16;
        int colorStep = 15;
        int frameStep = 1;
        var random = new Random();
        var points = new List<Point>();
        var recycled = new Stack<Point>();
        var frame = 0;
        while (IsRunning())
        {
            var start = Environment.TickCount64;
            frame++;
            if (frame % frameStep == 0)
            {
                if (recycled.Count == 0)
                    points.Add(new Point(random.Next(0, canvas.Width - 1), 0));
                else
                {
                    var point = recycled.Pop();
                    point.X = random.Next(0, canvas.Width - 1);
                    point.Y = 0;
                    point.Recycled = false;
                }
            }
            canvas.Clear();
            foreach (var point in points)
            {
                if (point.Recycled)
                    continue;
                point.Y++;
                if (point.Y - maxHeight > canvas.Height)
                {
                    point.Recycled = true;
                    recycled.Push(point);
                }
                for (var i = 0; i < maxHeight; i++)
                    canvas.SetPixel(point.X, point.Y - i, new(0, 255 - i * colorStep, 0));
            }
            matrix.SwapOnVSync(canvas);
            var elapsed = Environment.TickCount64 - start;
            if (elapsed < 33)
                Thread.Sleep(33 - (int)elapsed); // force 30 FPS (1000 / 30 = 33)
        }
    }
    #endregion

}