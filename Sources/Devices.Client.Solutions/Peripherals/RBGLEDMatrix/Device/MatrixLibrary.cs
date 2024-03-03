using System.Runtime.InteropServices;
using System.Security;

namespace Devices.Client.Solutions.Peripherals.RBGLEDMatrix.Device;

/// <summary>
/// Matrix library
/// </summary>
[SuppressUnmanagedCodeSecurity]
internal static partial class MatrixLibrary
{

    #region Constants
    private const string LIBRARY_NAME = "librgbmatrix.so.1";
    #endregion

    #region External Methods
    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_create", SetLastError = false)]
    internal static partial IntPtr CreateMatrix(int rows, int chained, int parallel);

    [LibraryImport(LIBRARY_NAME, EntryPoint = "led_matrix_create_from_options_const_argv", SetLastError = false, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial IntPtr CreateMatrixFromOptions(ref MatrixOptionsDevice options, int argc, [In] string[] argv);

    [LibraryImport(LIBRARY_NAME, EntryPoint = "led_matrix_create_from_options_and_rt_options", SetLastError = false)]
    internal static partial IntPtr CreateMatrixFromOptionsAndRuntimeOptions(ref MatrixOptionsDevice options, ref MatrixOptionsDeviceRuntime runtimeOptions);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_create_offscreen_canvas", SetLastError = false)]
    internal static partial IntPtr CreateOffscreenCanvas(IntPtr matrix);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_swap_on_vsync", SetLastError = false)]
    internal static partial IntPtr SwapOnVSync(IntPtr matrix, IntPtr canvas);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_delete", SetLastError = false)]
    internal static partial void DeleteMatrix(IntPtr matrix);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_get_canvas", SetLastError = false)]
    internal static partial IntPtr GetCanvas(IntPtr matrix);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_canvas_get_size", SetLastError = false), SuppressGCTransition]
    internal static partial void GetSize(IntPtr canvas, out int width, out int height);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_get_brightness", SetLastError = false), SuppressGCTransition]
    internal static partial byte GetBrightness(IntPtr matrix);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_matrix_set_brightness", SetLastError = false), SuppressGCTransition]
    internal static partial void SetBrightness(IntPtr matrix, byte brightness);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_canvas_fill", SetLastError = false)]
    internal static partial void Fill(IntPtr canvas, byte r, byte g, byte b);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_canvas_clear", SetLastError = false)]
    internal static partial void Clear(IntPtr canvas);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_canvas_set_pixel", SetLastError = false), SuppressGCTransition]
    internal static partial void SetPixel(IntPtr canvas, int x, int y, byte r, byte g, byte b);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "led_canvas_set_pixels", SetLastError = false)]
    internal static partial void SetPixels(IntPtr canvas, int x, int y, int width, int height, ref Color colors);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "set_image", SetLastError = false)]
    internal static partial void SetImage(IntPtr canvas, int x, int y, [In] byte[] buffer, uint bufferSize, int width, int height, byte is_bgr);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "draw_circle", SetLastError = false)]
    internal static partial void DrawCircle(IntPtr canvas, int x, int y, int radius, byte r, byte g, byte b);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "draw_line", SetLastError = false)]
    internal static partial void DrawLine(IntPtr canvas, int x0, int y0, int x1, int y1, byte r, byte g, byte b);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "load_font", SetLastError = false, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial IntPtr LoadFont(string bdf_font_file);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "delete_font", SetLastError = false)]
    internal static partial void DeleteFont(IntPtr font);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "height_font", SetLastError = false)]
    internal static partial int GetFontHeight(IntPtr font);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "baseline_font", SetLastError = false)]
    internal static partial int GetFontBaseline(IntPtr font);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "create_outline_font", SetLastError = false)]
    internal static partial IntPtr CreateOutlineFont(IntPtr font);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "draw_text", SetLastError = false, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial int DrawTextHorizontal(IntPtr canvas, IntPtr font, int x, int y, byte r, byte g, byte b, string utf8_text, int kerning_offset);

    [LibraryImport(libraryName: LIBRARY_NAME, EntryPoint = "vertical_draw_text", SetLastError = false, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
    internal static partial int DrawTextVertical(IntPtr canvas, IntPtr font, int x, int y, byte r, byte g, byte b, string utf8_text, int kerning_offset);
    #endregion

}