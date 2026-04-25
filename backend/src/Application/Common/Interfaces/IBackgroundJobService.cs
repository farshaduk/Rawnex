namespace Rawnex.Application.Common.Interfaces;

/// <summary>
/// Abstraction for enqueuing and scheduling background jobs.
/// </summary>
public interface IBackgroundJobService
{
    /// <summary>Enqueue a job to run immediately in the background.</summary>
    string Enqueue<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall);

    /// <summary>Schedule a job to run after a delay.</summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, TimeSpan delay);

    /// <summary>Schedule a job to run at a specific time.</summary>
    string Schedule<T>(System.Linq.Expressions.Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt);

    /// <summary>Create or update a recurring job.</summary>
    void AddOrUpdateRecurring<T>(string recurringJobId, System.Linq.Expressions.Expression<Func<T, Task>> methodCall, string cronExpression);

    /// <summary>Remove a recurring job.</summary>
    void RemoveRecurring(string recurringJobId);
}
