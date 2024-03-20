namespace Devices.Common.Tasks;

/// <summary>
/// Limited concurrency task scheduler
/// </summary>
public class LimitedConcurrencyTaskScheduler : TaskScheduler
{

    #region Private Fields
    private readonly int maximumConcurrencyLevel;
    private readonly LinkedList<Task> tasks = new();
    private volatile int tasksQueuedOrRunning = 0;
    [ThreadStatic]
    private static bool currentThreadIsProcessingItems;
    #endregion

    #region Properties
    /// <summary>
    /// Maximum concurrency level
    /// </summary>
    public sealed override int MaximumConcurrencyLevel { get => maximumConcurrencyLevel; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="maximumConcurrencyLevel"></param>
    public LimitedConcurrencyTaskScheduler(int maximumConcurrencyLevel)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(maximumConcurrencyLevel, 1);
        this.maximumConcurrencyLevel = maximumConcurrencyLevel;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return list of tasks queued to the scheduler waiting to be executed
    /// </summary>
    /// <returns></returns>
    protected override IEnumerable<Task> GetScheduledTasks()
    {
        bool lockTaken = false;
        try
        {
            Monitor.TryEnter(tasks, ref lockTaken);
            if (lockTaken)
                return tasks;
            else
                throw new NotSupportedException();
        }
        finally
        {
            if (lockTaken)
                Monitor.Exit(tasks);
        }
    }

    /// <summary>
    /// Queue task to the scheduler
    /// </summary>
    /// <param name="task"></param>
    protected override void QueueTask(Task task)
    {
        lock (tasks)
        {
            tasks.AddLast(task);
            if (tasksQueuedOrRunning < maximumConcurrencyLevel)
            {
                tasksQueuedOrRunning++;
                NotifyThreadPoolOfPendingWork();
            }
        }
    }

    /// <summary>
    /// Attempt to remove a previously scheduled task from the scheduler
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    protected sealed override bool TryDequeue(Task task)
    {
        lock (tasks)
            return tasks.Remove(task);
    }

    /// <summary>
    /// Determine whether the provided task can be executed synchronously in this call, and if it can, execute it.
    /// </summary>
    /// <param name="task"></param>
    /// <param name="taskWasPreviouslyQueued"></param>
    /// <returns></returns>
    protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
    {
        if (!currentThreadIsProcessingItems)
            return false;
        if (taskWasPreviouslyQueued)
            if (TryDequeue(task))
                return TryExecuteTask(task);
            else
                return false;
        return TryExecuteTask(task);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Notify the ThreadPool that there is work to be executed for this scheduler
    /// </summary>
    private void NotifyThreadPoolOfPendingWork()
    {
        ThreadPool.UnsafeQueueUserWorkItem(_ =>
        {
            currentThreadIsProcessingItems = true;
            try
            {
                while (true)
                {
                    Task item;
                    lock (tasks)
                    {
                        if (tasks.Count == 0)
                        {
                            tasksQueuedOrRunning--;
                            break;
                        }
                        item = tasks.First!.Value;
                        tasks.RemoveFirst();
                    }
                    TryExecuteTask(item);
                }
            }
            finally
            {
                currentThreadIsProcessingItems = false;
            }
        }, null);
    }
    #endregion

}