using CommandLine;
using Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.RBGLEDMatrix;

/// <summary>
/// RGB LED Matrix controller
/// </summary>
public abstract class RBGLEDMatrixController : PeripheralsController
{

    #region Properties
    /// <summary>
    /// LED matrix rows
    /// </summary>
    [Option('r', "rows", Required = false, Default = 64, HelpText = "RGB LED Matrix rows.")]
    public int Rows { get; set; }

    /// <summary>
    /// LED matrix columns
    /// </summary>
    [Option('c', "columns", Required = false, Default = 64, HelpText = "RGB LED Matrix columns.")]
    public int Columns { get; set; }

    /// <summary>
    /// LED matrix horizontal panels
    /// </summary>
    [Option('h', "horizontalPanels", Required = false, Default = 1, HelpText = "RGB LED Matrix horizontal panels.")]
    public int HorizontalPanels { get; set; }

    /// <summary>
    /// LED matrix vertical panels
    /// </summary>
    [Option('v', "verticalPanels", Required = false, Default = 1, HelpText = "RGB LED Matrix vertical panels.")]
    public int VerticalPanels { get; set; }

    /// <summary>
    /// LED matrix brightness
    /// </summary>
    [Option('b', "brightness", Required = false, Default = (byte)100, HelpText = "RGB LED Matrix brightness.")]
    public byte Brightness { get; set; }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return matrix instance
    /// </summary>
    /// <returns></returns>
    protected Matrix GetMatrix()
    {
        DisplayService.WriteInformation($"Press [Enter] to exit.");
        return new(new() { Rows = Rows, Columns = Columns, Chains = HorizontalPanels, ParallelChains = VerticalPanels }) { Brightness = Brightness };
    }

    /// <summary>
    /// Return font instance
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    protected static Font GetFont(string path) => new(path);
    #endregion

}