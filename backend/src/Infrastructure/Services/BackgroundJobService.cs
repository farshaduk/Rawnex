using System.Linq.Expressions;
using Hangfire;
using Rawnex.Application.Common.Interfaces;

namespace Rawnex.Infrastructure.Services;

public class BackgroundJobService : IBackgroundJobService
{
    public string Enqueue<T>(Expression<Func<T, Task>> methodCall)
    {
        return BackgroundJob.Enqueue(methodCall);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, TimeSpan delay)
    {
        return BackgroundJob.Schedule(methodCall, delay);
    }

    public string Schedule<T>(Expression<Func<T, Task>> methodCall, DateTimeOffset enqueueAt)
    {
        return BackgroundJob.Schedule(methodCall, enqueueAt);
    }

    public void AddOrUpdateRecurring<T>(string recurringJobId, Expression<Func<T, Task>> methodCall, string cronExpression)
    {
        RecurringJob.AddOrUpdate(recurringJobId, methodCall, cronExpression);
    }

    public void RemoveRecurring(string recurringJobId)
    {
        RecurringJob.RemoveIfExists(recurringJobId);
    }
}
