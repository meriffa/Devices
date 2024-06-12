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
                dynamic cameraController = scope.Get("CameraController").Invoke(args, Py.kw("displayDateTime", true));
                file = MemoryMappedFile.CreateFromFile($"/dev/shm/{cameraController.GetSharedMemoryName().As<string>()}", FileMode.Open, null, size);
                cameraController.Start();
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
    /// Send stop request
    /// </summary>
    public void SendStopRequest()
    {
        using (var accessor = file!.CreateViewAccessor())
        {
            accessor.Read(0, out CameraControlBlock cameraControlBlock);
            cameraControlBlock.StopRequest = true;
            accessor.Write(0, ref cameraControlBlock);
        }
        task.Wait();
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