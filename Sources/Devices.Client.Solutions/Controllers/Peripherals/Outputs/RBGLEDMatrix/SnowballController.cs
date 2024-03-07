using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// Snowball controller
/// </summary>
[Verb("RBGLEDMatrix-Snowball", HelpText = "RGB LED Matrix Snowball operation.")]
public class SnowballController : RBGLEDMatrixController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED Matrix Snowball operation started.");
        using var matrix = GetMatrix();
        DisplayCanvas(matrix);
        DisplayService.WriteInformation("RGB LED Matrix Snowball operation completed.");
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
        canvas.Fill(Color.DarkGray);
        var centerX = canvas.Width / 2;
        var centerY = canvas.Height / 2;
        var maxRadius = Math.Min(canvas.Width / 2.0f, canvas.Height / 2.0f);
        var angleStep = 1.0f / 360;
        while (IsRunning())
        {
            for (float angle = 0, radius = 0; radius < maxRadius; angle += angleStep, radius += angleStep)
                if (!DisplaySnowball(canvas, centerX, centerY, angle, radius, Color.Red))
                    return;
            for (float angle = 0, radius = maxRadius + 1; radius > 0; angle += angleStep, radius -= angleStep)
                if (!DisplaySnowball(canvas, centerX, centerY, angle, radius, Color.DarkGray))
                    return;
        }
    }

    /// <summary>
    /// Display snowball
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="centerX"></param>
    /// <param name="centerY"></param>
    /// <param name="radius_max"></param>
    /// <param name="angle_step"></param>
    /// <param name="color"></param>
    /// <returns></returns>
    private static bool DisplaySnowball(Canvas canvas, int centerX, int centerY, float angle, float radius, Color color)
    {
        if (!IsRunning())
            return false;
        var dotX = (int)Math.Round(Math.Cos(angle * 2 * Math.PI) * radius);
        var dotY = (int)Math.Round(Math.Sin(angle * 2 * Math.PI) * radius);
        canvas.SetPixel(centerX + dotX, centerY + dotY, color);
        Thread.Sleep(1);
        return true;
    }
    #endregion

}