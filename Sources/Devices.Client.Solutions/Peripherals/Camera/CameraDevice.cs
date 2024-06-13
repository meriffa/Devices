using Devices.Common.Extensions;
using Devices.Common.Solutions.Garden.Models;
using Python.Runtime;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.Camera;

/// <summary>
/// Camera device
/// </summary>
/// <param name="cameraDefinition"></param>
public sealed class CameraDevice(CameraDefinition cameraDefinition) : IDisposable
{

    #region Private Fields
    private readonly CameraDefinition cameraDefinition = cameraDefinition;
    private readonly EventWaitHandle initialized = new(false, EventResetMode.ManualReset);
    private Task? task;
    private MemoryMappedFile? file;
    #endregion

    #region Public Methods
    /// <summary>
    /// Start camera
    /// </summary>
    public void Start()
    {
        Exception? exception = null;
        task = Task.Run(() =>
        {
            try
            {
                PythonEngine.Initialize();
                using var gil = Py.GIL();
                using var scope = Py.CreateScope();
                dynamic sys = Py.Import("sys");
                sys.path.append($"{AppDomain.CurrentDomain.BaseDirectory}Python");
                var cameraModuleFile = $"{AppDomain.CurrentDomain.BaseDirectory}Python/CameraController.py";
                using var _ = scope.Execute(PythonEngine.Compile(File.ReadAllText(cameraModuleFile), cameraModuleFile));
                int size = Marshal.SizeOf(typeof(CameraControlBlock));
                using var cameraController = scope.Get("CameraController").Invoke(new PyObject[]
                {
                    size.ToPython(),
                    cameraDefinition.Source.ToPython(),
                    cameraDefinition.Width.ToPython(),
                    cameraDefinition.Height.ToPython(),
                    cameraDefinition.FramesPerSecond.ToPython(),
                    cameraDefinition.Bitrate.ToPython(),
                    cameraDefinition.PublishLocation.ToPython()
                }, Py.kw("displayDateTime", true));
                file = MemoryMappedFile.CreateFromFile($"/dev/shm/{cameraController.InvokeMethod("GetSharedMemoryName").As<string>()}", FileMode.Open, null, size);
                initialized.Set();
                cameraController.InvokeMethod("Start");
            }
            catch (Exception ex)
            {
                exception = ex;
                initialized.Set();
            }
        });
        initialized.WaitOne();
        if (exception != null)
            throw new($"Camera device initialization failed (Error = '{exception.Message}').");
    }

    /// <summary>
    /// Stop camera
    /// </summary>
    public void Stop()
    {
        if (task == null || file == null)
            throw new("Camera device not started.");
        using (var accessor = file.CreateViewAccessor())
        {
            accessor.Read(0, out CameraControlBlock cameraControlBlock);
            cameraControlBlock.StopRequest = true;
            accessor.Write(0, ref cameraControlBlock);
        }
        task.Wait();
    }

    /// <summary>
    /// Get camera focus range
    /// </summary>
    /// <returns></returns>
    public (double, double) GetFocusRange()
    {
        if (file == null)
            throw new("Camera device not started.");
        using var accessor = file.CreateViewAccessor();
        accessor.Read(0, out CameraControlBlock cameraControlBlock);
        cameraControlBlock.FocusRangeRequest = true;
        accessor.Write(0, ref cameraControlBlock);
        accessor.Flush();
        do
        {
            DelayExtension.DelayMicroseconds(100_000, allowThreadYield: true);
            accessor.Read(0, out cameraControlBlock);
        } while (cameraControlBlock.FocusRangeRequest);
        return (cameraControlBlock.FocusMinimum / 100.0d, cameraControlBlock.FocusMaximum / 100.0d);
    }

    /// <summary>
    /// Set camera focus
    /// </summary>
    /// <param name="value"></param>
    public void SetFocus(double value)
    {
        if (file == null)
            throw new("Camera device not started.");
        using var accessor = file.CreateViewAccessor();
        accessor.Read(0, out CameraControlBlock cameraControlBlock);
        cameraControlBlock.FocusValue = (int)Math.Round(value * 100);
        cameraControlBlock.FocusRequest = true;
        accessor.Write(0, ref cameraControlBlock);
    }

    /// <summary>
    /// Set camera zoom
    /// </summary>
    /// <param name="value"></param>
    public void SetZoom(double value)
    {
        if (file == null)
            throw new("Camera device not started.");
        using var accessor = file.CreateViewAccessor();
        accessor.Read(0, out CameraControlBlock cameraControlBlock);
        cameraControlBlock.ZoomValue = (int)Math.Round(value * 100);
        cameraControlBlock.ZoomRequest = true;
        accessor.Write(0, ref cameraControlBlock);
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        file?.Dispose();
        file = null;
    }
    #endregion

}