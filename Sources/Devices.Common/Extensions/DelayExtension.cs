using System.Diagnostics;

namespace Devices.Common.Extensions;

/// <summary>
/// Delay methods
/// </summary>
public static class DelayExtension
{

    #region Constants
    private const long TICKS_PER_SECOND = TimeSpan.TicksPerSecond;
    private const long TICKS_PER_MILLISECOND = TimeSpan.TicksPerMillisecond;
    private const long TICKS_PER_MICROSECOND = TimeSpan.TicksPerMillisecond / 1000;
    private static readonly double TICK_FREQUENCY = (double)TICKS_PER_SECOND / Stopwatch.Frequency;
    #endregion

    #region Public Methods
    /// <summary>
    /// Delay for at least the specified <paramref name="time" />.
    /// </summary>
    /// <param name="time">The amount of time to delay.</param>
    /// <param name="allowThreadYield">True to allow yielding the thread. If this is set to false, on single-proc systems this will prevent all other code from running.</param>
    public static void Delay(TimeSpan time, bool allowThreadYield)
    {
        long start = Stopwatch.GetTimestamp();
        long delta = (long)(time.Ticks / TICK_FREQUENCY);
        long target = start + delta;
        if (!allowThreadYield)
        {
            do
                Thread.SpinWait(1);
            while (Stopwatch.GetTimestamp() < target);
        }
        else
        {
            var spinWait = new SpinWait();
            do
                spinWait.SpinOnce();
            while (Stopwatch.GetTimestamp() < target);
        }
    }

    /// <summary>
    /// Delay for at least the specified <paramref name="microseconds"/>.
    /// </summary>
    /// <param name="microseconds">The number of microseconds to delay.</param>
    /// <param name="allowThreadYield">True to allow yielding the thread. If this is set to false, on single-proc systems this will prevent all other code from running.</param>
    public static void DelayMicroseconds(int microseconds, bool allowThreadYield) => Delay(TimeSpan.FromTicks(microseconds * TICKS_PER_MICROSECOND), allowThreadYield);

    /// <summary>
    /// Delay for at least the specified <paramref name="milliseconds"/>
    /// </summary>
    /// <param name="milliseconds">The number of milliseconds to delay.</param>
    /// <param name="allowThreadYield">True to allow yielding the thread. If this is set to false, on single-proc systems this will prevent all other code from running.</param>
    public static void DelayMilliseconds(int milliseconds, bool allowThreadYield) => Delay(TimeSpan.FromTicks(milliseconds * TICKS_PER_MILLISECOND), allowThreadYield);
    #endregion

}