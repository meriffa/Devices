using Devices.Common.Extensions;
using Devices.Common.Solutions.Garden.Models;
using Python.Runtime;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;

namespace Devices.Client.Solutions.Peripherals.Camera;

/// <summary>
/// Raspberry Pi camera module (V2, HQ, V3)
/// </summary>
public sealed class RaspberryPiCameraModule : IDisposable
{

    #region Private Fields
    private Py.GILState? gil;
    private PyModule? scope;
    private MemoryMappedFile? file;
    private readonly Task task;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="fps"></param>
    /// <param name="location"></param>
    public RaspberryPiCameraModule(int width, int height, int fps, string location)
    {
        task = Task.Run(() =>
        {
            try
            {
                PythonEngine.Initialize();
                PythonEngine.BeginAllowThreads();
                gil = Py.GIL();
                scope = Py.CreateScope();
                dynamic sys = Py.Import("sys");
                sys.path.append($"{AppDomain.CurrentDomain.BaseDirectory}Python");
                var cameraModuleFile = $"{AppDomain.CurrentDomain.BaseDirectory}Python/CameraController.py";
                scope.Execute(PythonEngine.Compile(File.ReadAllText(cameraModuleFile), cameraModuleFile));
                int size = Marshal.SizeOf(typeof(CameraControlBlock));
                var args = new PyObject[] { size.ToPython(), "PiCSI".ToPython(), width.ToPython(), height.ToPython(), fps.ToPython(), location.ToPython() };
                var cameraController = scope.Get("CameraController").Invoke(args, Py.kw("displayDateTime", true));
                file = MemoryMappedFile.CreateFromFile($"/dev/shm/{cameraController.InvokeMethod("GetSharedMemoryName").As<string>()}", FileMode.Open, null, size);
                cameraController.InvokeMethod("Start");
            }
            catch (Exception ex)
            {
                throw new($"Raspberry Pi camera module initialization failed (Error = '{ex.Message}').");
            }
        });
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Stop camera
    /// </summary>
    public void Stop()
    {
        using (var accessor = file!.CreateViewAccessor())
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
        using var accessor = file!.CreateViewAccessor();
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
        using var accessor = file!.CreateViewAccessor();
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
        using var accessor = file!.CreateViewAccessor();
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
        PythonEngine.Shutdown();
        file?.Dispose();
        file = null;
        scope?.Dispose();
        scope = null;
        gil?.Dispose();
        gil = null;
    }
    #endregion

}