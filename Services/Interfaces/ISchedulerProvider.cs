using System.Reactive.Concurrency;

namespace Services.Interfaces
{
    public interface ISchedulerProvider
    {
        IScheduler Default { get; }
    }
}