using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Genetics controller
/// </summary>
[Verb("RBGLEDMatrix-Genetics", HelpText = "RGB LED Matrix Genetics operation.")]
public class GeneticsController : RBGLEDMatrixController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"RGB LED Matrix Genetics operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation($"RGB LED Matrix Genetics operation completed.");
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
        var torus = true;
        var width = canvas.Width;
        var height = canvas.Height;
        var originalValues = new int[width][];
        var random = new Random();
        for (int x = 0; x < width; ++x)
            originalValues[x] = new int[height];
        var newValues = new int[width][];
        for (int x = 0; x < width; ++x)
            newValues[x] = new int[height];
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                originalValues[x][y] = random.Next(2);
        int r = random.Next(255);
        int g = random.Next(255);
        int b = random.Next(255);
        if (r < 150 && g < 150 && b < 150)
            switch (random.Next(3))
            {
                case 0:
                    r = 200;
                    break;
                case 1:
                    g = 200;
                    break;
                case 2:
                    b = 200;
                    break;
            }
        while (IsRunning())
        {
            UpdateValues(torus, width, height, originalValues, newValues);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    canvas.SetPixel(x, y, originalValues[x][y] != 0 ? new(r, g, b) : Color.Black);
            Thread.Sleep(50);
        }
    }

    /// <summary>
    /// Update population values
    /// </summary>
    /// <param name="torus"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="originalValues"></param>
    /// <param name="newValues"></param>
    private static void UpdateValues(bool torus, int width, int height, int[][] originalValues, int[][] newValues)
    {
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                newValues[x][y] = originalValues[x][y];
        // update newValues based on values
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                int num = GetNumberOfAliveNeighbors(torus, width, height, originalValues, x, y);
                if (originalValues[x][y] != 0)
                {
                    // Cell is alive
                    if (num < 2 || num > 3)
                        newValues[x][y] = 0;
                }
                else
                {
                    // Cell is dead
                    if (num == 3)
                        newValues[x][y] = 1;
                }
            }
        }
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
                originalValues[x][y] = newValues[x][y];
    }

    /// <summary>
    /// Return number of alive neighbors
    /// </summary>
    /// <param name="torus"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="originalValues"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    private static int GetNumberOfAliveNeighbors(bool torus, int width, int height, int[][] originalValues, int x, int y)
    {
        int result = 0;
        if (torus)
        {
            // Edges are connected (torus)
            result += originalValues[(x - 1 + width) % width][(y - 1 + height) % height];
            result += originalValues[(x - 1 + width) % width][y];
            result += originalValues[(x - 1 + width) % width][(y + 1) % height];
            result += originalValues[(x + 1) % width][(y - 1 + height) % height];
            result += originalValues[(x + 1) % width][y];
            result += originalValues[(x + 1) % width][(y + 1) % height];
            result += originalValues[x][(y - 1 + height) % height];
            result += originalValues[x][(y + 1) % height];
        }
        else
        {
            // Edges are not connected (no torus)
            if (x > 0)
            {
                if (y > 0)
                    result += originalValues[x - 1][y - 1];
                if (y < height - 1)
                    result += originalValues[x - 1][y + 1];
                result += originalValues[x - 1][y];
            }
            if (x < width - 1)
            {
                if (y > 0)
                    result += originalValues[x + 1][y - 1];
                if (y < 31)
                    result += originalValues[x + 1][y + 1];
                result += originalValues[x + 1][y];
            }
            if (y > 0)
                result += originalValues[x][y - 1];
            if (y < height - 1)
                result += originalValues[x][y + 1];
        }
        return result;
    }
    #endregion

}