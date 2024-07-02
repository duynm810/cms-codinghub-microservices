namespace Shared.Helpers;

public static class TaskHelper
{
    /// <summary>
    /// Runs a task in a fire-and-forget manner, ensuring exceptions are handled.
    /// </summary>
    /// <param name="task">The asynchronous task to run.</param>
    /// <param name="exceptionHandler">An optional exception handler to process any exceptions thrown by the task.</param>
    public static void RunFireAndForget(Func<Task> task, Action<Exception>? exceptionHandler = null)
    {
        Task.Run(async () =>
        {
            try
            {
                await task().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Log or handle exceptions (Log hoặc xử lý ngoại lệ)
                exceptionHandler?.Invoke(ex);
            }
        }).ContinueWith(t =>
        {
            if (t.Exception != null)
            {
                // Handle unobservable exceptions (Xử lý ngoại lệ không quan sát được)
                exceptionHandler?.Invoke(t.Exception);
            }
        }, TaskContinuationOptions.OnlyOnFaulted);
    }
}