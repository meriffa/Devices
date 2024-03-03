using System.Diagnostics;

namespace Devices.Client.Solutions.Peripherals.FFmpeg;

/// <summary>
/// FFmpeg player
/// </summary>
/// <remarks>
/// Initialization
/// </remarks>
/// <param name="path"></param>
public class FFmpegPlayer(string path, int width, int height, bool loop)
{

    #region Constants
    private const int BUFFER_SIZE = 1_920 * 1_080 * 4;
    #endregion

    #region Private Fields
    private readonly string path = path;
    private readonly int width = width;
    private readonly int height = height;
    private readonly bool loop = loop;
    private Task? task;
    private readonly CancellationTokenSource cancellationTokenSource = new();
    #endregion

    #region Delegates
    /// <summary>
    /// Render frame event action
    /// </summary>
    /// <param name="frame"></param>
    public delegate void OnRenderFrameAction(ReadOnlySpan<byte> frame);
    #endregion

    #region Events
    /// <summary>
    /// Render frame event
    /// </summary>
    public event OnRenderFrameAction? OnRenderFrame;

    /// <summary>
    /// Playback completed event
    /// </summary>
    public event Action? OnPlaybackCompleted;
    #endregion

    #region Public Methods
    /// <summary>
    /// Start player
    /// </summary>
    public void Start()
    {
        task = Task.Factory.StartNew(PlaybackVideo, cancellationTokenSource.Token);
    }

    /// <summary>
    /// Stop player
    /// </summary>
    public void Stop()
    {
        cancellationTokenSource.Cancel();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Video playback
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void PlaybackVideo()
    {
        var bufferIndex = 0;
        var buffer = new byte[BUFFER_SIZE];
        using var process = GetProcess();
        using var stream = process.StandardOutput.BaseStream;
        while (!process.HasExited && !cancellationTokenSource.IsCancellationRequested)
        {
            if (!ParseHeader(stream, buffer, ref bufferIndex))
                break;
            while (true)
            {
                (var length, var type) = ParseChunkHeader(stream, buffer, ref bufferIndex);
                if (length > 0)
                {
                    int totalRead = 0;
                    while (totalRead < length)
                    {
                        int bytesRead = stream.Read(buffer, bufferIndex, length - totalRead);
                        bufferIndex += bytesRead;
                        totalRead += bytesRead;
                    }
                }
                else if (length == 0 && type == 0x444E4549)
                    break;
            }
            OnRenderFrame?.Invoke(new ReadOnlySpan<byte>(buffer, 0, bufferIndex));
        }
        if (!process.HasExited)
            process.Kill();
        OnPlaybackCompleted?.Invoke();
    }

    /// <summary>
    /// Return FFmpeg process
    /// </summary>
    /// <returns></returns>
    private Process GetProcess()
    {
        var process = new Process()
        {
            StartInfo = new()
            {
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel error -stream_loop {(loop ? -1 : 0)} -i \"{path}\" -codec:v png -f image2pipe -vf \"scale={width}:{height}\" -",
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true
            },
            EnableRaisingEvents = true
        };
        process.Start();
        return process;
    }

    /// <summary>
    /// Parse PNG header
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="buffer"></param>
    /// <param name="bufferIndex"></param>
    /// <returns></returns>
    private static bool ParseHeader(Stream stream, byte[] buffer, ref int bufferIndex)
    {
        var bytesRead = stream.Read(buffer, bufferIndex = 0, 8);
        if (bytesRead == 0)
            return false;
        if (bytesRead != 8 || BitConverter.ToUInt64(buffer, 0) != 0x0A1A0A0D474E5089)
            throw new("Invalid PNG signature.");
        bufferIndex += 8;
        return true;
    }

    /// <summary>
    /// Parse PNG chunk header
    /// </summary>
    /// <param name="stream"></param>
    /// <param name="buffer"></param>
    /// <param name="bufferIndex"></param>
    /// <returns></returns>
    private static (int length, int type) ParseChunkHeader(Stream stream, byte[] buffer, ref int bufferIndex)
    {
        var index = bufferIndex;
        var bytesRead = stream.Read(buffer, bufferIndex, 12);
        if (bytesRead != 12)
            throw new("Invalid PNG chunk header.");
        bufferIndex += 12;
        var length = BitConverter.ToInt32(new ReadOnlySpan<byte>(buffer.Skip(index).Take(4).Reverse().ToArray()));
        var type = BitConverter.ToInt32(buffer, index + 4);
        return (length, type);
    }
    #endregion

}