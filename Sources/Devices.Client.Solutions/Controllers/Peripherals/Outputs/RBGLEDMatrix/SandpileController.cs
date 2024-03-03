using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Sandpile controller
/// </summary>
[Verb("RBGLEDMatrix-Sandpile", HelpText = "RGB LED Matrix Sandpile operation.")]
public class SandpileController : RBGLEDMatrixController
{

    #region Constants
    private static readonly Color[] Colors = [new Color(0, 0, 0), new Color(0, 0, 200), new Color(0, 200, 0), new Color(150, 100, 0), new Color(200, 0, 0)];
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"RGB LED Matrix Sandpile operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation($"RGB LED Matrix Sandpile operation completed.");
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
        var width = canvas.Width - 1; // odd width required
        var height = canvas.Height - 1; // odd height required
        var originalValues = new int[width][];
        for (int x = 0; x < width; ++x)
            originalValues[x] = new int[height];
        var newValues = new int[width][];
        for (int x = 0; x < width; ++x)
            newValues[x] = new int[height];
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                originalValues[x][y] = 0;
        while (IsRunning())
        {
            originalValues[width / 2][height / 2]++; // Drop a sand grain in the centre
            UpdateValues(originalValues, newValues, width, height);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    canvas.SetPixel(x, y, originalValues[x][y] < Colors.Length ? Colors[originalValues[x][y]] : Colors[^1]);
            Thread.Sleep(50);
        }
    }

    /// <summary>
    /// Update pile values
    /// </summary>
    /// <param name="originalValues"></param>
    /// <param name="newValues"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    private static void UpdateValues(int[][] originalValues, int[][] newValues, int width, int height)
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                newValues[x][y] = originalValues[x][y];
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                if (originalValues[x][y] > 3)
                {
                    if (x > 0)
                        newValues[x - 1][y]++;
                    if (x < width - 1)
                        newValues[x + 1][y]++;
                    if (y > 0)
                        newValues[x][y - 1]++;
                    if (y < height - 1)
                        newValues[x][y + 1]++;
                    newValues[x][y] -= 4;
                }
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                originalValues[x][y] = newValues[x][y];
    }
    #endregion

}