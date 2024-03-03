using CommandLine;
using Devices.Client.Solutions.Peripherals.EPaper.Common;
using Devices.Client.Solutions.Peripherals.EPaper.Images.ImageSharp;
using Devices.Client.Solutions.Peripherals.EPaper.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs;

/// <summary>
/// e-Paper Display controller
/// </summary>
[Verb("Outputs-EPaperDisplay", HelpText = "e-Paper Display operation.")]
public class EPaperDisplayController : PeripheralsController
{

    #region Properties
    /// <summary>
    /// Image file name
    /// </summary>
    [Option('i', "image", Required = true, HelpText = "Image file name.")]
    public string ImageFileName { get; set; } = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"e-Paper Display operation started.");
        if (!File.Exists(ImageFileName))
            throw new($"Image file '{ImageFileName}' not found.");
        using var display = SetupDisplay();
        ClearDisplay(display);
        DisplayImage(display, ImageFileName);
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        ClearDisplay(display);
        SleepDisplay(display);
        DisplayService.WriteInformation($"e-Paper Display operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup display
    /// </summary>
    /// <returns></returns>
    private IImageDevice<Image<Rgba32>> SetupDisplay()
    {
        var display = DisplayFactory<Rgba32>.Create(DisplayType.Waveshare75B);
        DisplayService.WriteInformation($"e-Paper Display initialized.");
        return display;
    }

    /// <summary>
    /// Clear display
    /// </summary>
    /// <param name="display"></param>
    private void ClearDisplay(IImageDevice<Image<Rgba32>> display)
    {
        display.Clear();
        DisplayService.WriteInformation($"e-Paper Display cleared.");
    }

    /// <summary>
    /// Display image
    /// </summary>
    /// <param name="display"></param>
    /// <param name="path"></param>
    private void DisplayImage(IImageDevice<Image<Rgba32>> display, string path)
    {
        using var image = Image.Load<Rgba32>(path);
        display.DisplayImage(image);
        DisplayService.WriteInformation($"e-Paper Display image '{path}' displayed.");
    }

    /// <summary>
    /// Sleep display
    /// </summary>
    /// <param name="display"></param>
    private void SleepDisplay(IImageDevice<Image<Rgba32>> display)
    {
        display.Sleep();
        DisplayService.WriteInformation($"e-Paper Display deep sleep mode on.");
    }
    #endregion

}