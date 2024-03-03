using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Ant controller
/// </summary>
[Verb("RBGLEDMatrix-Ant", HelpText = "RGB LED Matrix Ant operation.")]
public class AntController : RBGLEDMatrixController
{

    #region Constants
    private static readonly Color[] Colors = [new Color(200, 0, 0), new Color(0, 200, 0), new Color(0, 0, 200), new Color(150, 100, 0)];
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"RGB LED Matrix Ant operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation($"RGB LED Matrix Ant operation completed.");
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
        int width = canvas.Width;
        int height = canvas.Height;
        int[][] values = new int[width][];
        for (int x = 0; x < width; ++x)
            values[x] = new int[height];
        int antX = width / 2;
        int antY = height / 2 - 3;
        int antDirection = 0; // 0 = Right, 1 = Up, 2 = Left, 3 = Down
        for (int x = 0; x < width; ++x)
            for (int y = 0; y < height; ++y)
            {
                values[x][y] = 0;
                UpdatePixel(canvas, values, x, y, x, y);
            }
        while (IsRunning())
        {
            antDirection = values[antX][antY] switch
            {
                0 or 1 => (antDirection + 1 + 4) % 4,
                _ => (antDirection - 1 + 4) % 4
            };
            values[antX][antY] = (values[antX][antY] + 1) % Colors.Length;
            int previousX = antX;
            int previousY = antY;
            switch (antDirection)
            {
                case 0:
                    antX++;
                    break;
                case 1:
                    antY++;
                    break;
                case 2:
                    antX--;
                    break;
                case 3:
                    antY--;
                    break;
            }
            UpdatePixel(canvas, values, previousX, previousY, antX, antY);
            if (antX < 0 || antX >= width || antY < 0 || antY >= height)
                return;
            UpdatePixel(canvas, values, antX, antY, antX, antY);
            Thread.Sleep(100);
        }
    }

    /// <summary>
    /// Update ant pixel
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="values"></param>
    /// <param name="currentX"></param>
    /// <param name="currentY"></param>
    /// <param name="previousX"></param>
    /// <param name="previousY"></param>
    private static void UpdatePixel(Canvas canvas, int[][] values, int currentX, int currentY, int previousX, int previousY)
    {
        canvas.SetPixel(currentX, currentY, Colors[values[currentX][currentY]]);
        if (currentX == previousX && currentY == previousY)
            canvas.SetPixel(currentX, currentY, Color.Black);
    }
    #endregion

}