using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Equalizer controller
/// </summary>
[Verb("RBGLEDMatrix-Equalizer", HelpText = "RGB LED Matrix Equalizer operation.")]
public class EqualizerController : RBGLEDMatrixController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Equalizer operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Equalizer operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Display canvas
    /// </summary>
    /// <param name="matrix"></param>
    private static void DisplayCanvas(Matrix matrix)
    {
        var canvas = matrix.GetCanvas();
        var width = canvas.Width;
        var height = canvas.Height;
        var barCount = canvas.Width / 2;
        var barWidth = width / barCount;
        var barHeights = new int[barCount];
        var barMeans = new int[barCount];
        var barFrequencies = new int[barCount];
        var heightGreen = height * 4 / 12;
        var heightYellow = height * 8 / 12;
        var heightOrange = height * 10 / 12;
        var heightRed = height * 12 / 12;
        var t = 0;
        // Array of possible bar means
        var means = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 16, 32 };
        for (int i = 0; i < means.Length; ++i)
            means[i] = height - means[i] * height / 8;
        // Initialize bar means
        var random = new Random();
        for (int i = 0; i < barCount; ++i)
        {
            barMeans[i] = random.Next(means.Length);
            barFrequencies[i] = 1 << random.Next(3);
        }
        // Display bars
        while (IsRunning())
        {
            if (t % 8 == 0)
            {
                // Change the means
                for (int i = 0; i < barCount; ++i)
                {
                    barMeans[i] += random.Next(3) - 1;
                    if (barMeans[i] >= means.Length)
                        barMeans[i] = means.Length - 1;
                    if (barMeans[i] < 0)
                        barMeans[i] = 0;
                }
            }
            // Update bar heights
            t++;
            for (int i = 0; i < barCount; ++i)
            {
                barHeights[i] = (int)Math.Round((height - means[barMeans[i]]) * Math.Sin(0.1d * t * barFrequencies[i]) + means[barMeans[i]]);
                if (barHeights[i] < height / 8)
                    barHeights[i] = random.Next(height / 8) + 1;
            }
            for (int i = 0, y; i < barCount; ++i)
            {
                for (y = 0; y < barHeights[i]; ++y)
                    if (y < heightGreen)
                        DrawBarRow(canvas, barWidth, height, i, y, Color.Green);
                    else if (y < heightYellow)
                        DrawBarRow(canvas, barWidth, height, i, y, Color.Yellow);
                    else if (y < heightOrange)
                        DrawBarRow(canvas, barWidth, height, i, y, Color.Orange);
                    else if (y < heightRed)
                        DrawBarRow(canvas, barWidth, height, i, y, Color.Red);
                for (; y < height; ++y)
                    DrawBarRow(canvas, barWidth, height, i, y, Color.Black); // Anything above the bar should be black
            }
            Thread.Sleep(25);
        }
    }

    /// <summary>
    /// Draw bar row
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="barWidth"></param>
    /// <param name="height"></param>
    /// <param name="bar"></param>
    /// <param name="y"></param>
    /// <param name="color"></param>
    private static void DrawBarRow(Canvas canvas, int barWidth, int height, int bar, int y, Color color)
    {
        for (int x = bar * barWidth; x < (bar + 1) * barWidth; ++x)
            canvas.SetPixel(x, height - 1 - y, color);
    }
    #endregion

}