using System.Linq.Expressions;

namespace Contracts.Scheduled;

public interface IScheduledJobService
{
    #region Continuos Jobs

    /// <summary>
    /// Enqueue a job to run after a parent job completes.
    /// </summary>
    /// <param name="parentJobId">The ID of the parent job.</param>
    /// <param name="functionCall">The method to call after the parent job completes.</param>
    /// <returns>The ID of the enqueued job.</returns>
    string ContinueQueueWith(string parentJobId, Expression<Action> functionCall);

    #endregion

    #region Fire And Forget

    /// <summary>
    /// Enqueue a fire-and-forget job.
    /// </summary>
    /// <param name="functionCall">The method to call.</param>
    /// <returns>The ID of the enqueued job.</returns>
    string Enqueue(Expression<Action> functionCall);

    /// <summary>
    /// Enqueue a fire-and-forget job with a parameter.
    /// </summary>
    /// <param name="functionCall">The method to call with a parameter.</param>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <returns>The ID of the enqueued job.</returns>
    string Enqueue<T>(Expression<Action<T>> functionCall);

    #endregion

    #region Delayed Jobs

    /// <summary>
    /// Schedule a job to run after a delay.
    /// </summary>
    /// <param name="functionCall">The method to call.</param>
    /// <param name="delay">The delay after which the job will run.</param>
    /// <returns>The ID of the scheduled job.</returns>
    string Schedule(Expression<Action> functionCall, TimeSpan delay);

    /// <summary>
    /// Schedule a job with a parameter to run after a delay.
    /// </summary>
    /// <param name="functionCall">The method to call with a parameter.</param>
    /// <param name="delay">The delay after which the job will run.</param>
    /// <typeparam name="T">The type of the parameter.</typeparam>
    /// <returns>The ID of the scheduled job.</returns>
    string Schedule<T>(Expression<Action<T>> functionCall, TimeSpan delay);

    /// <summary>
    /// Schedule a job to run at a specific time.
    /// </summary>
    /// <param name="functionCall">The method to call.</param>
    /// <param name="enqueueAt">The time at which the job will run.</param>
    /// <returns>The ID of the scheduled job.</returns>
    string Schedule(Expression<Action> functionCall, DateTimeOffset enqueueAt);

    #endregion

    /// <summary>
    /// Delete a job by its ID.
    /// </summary>
    /// <param name="jobId">The ID of the job to delete.</param>
    /// <returns>True if the job was deleted successfully.</returns>
    bool Delete(string jobId);

    /// <summary>
    /// Requeue a job by its ID.
    /// </summary>
    /// <param name="jobId">The ID of the job to requeue.</param>
    /// <returns>True if the job was requeued successfully.</returns>
    bool Requeue(string jobId);
}