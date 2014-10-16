using System.Reactive.Concurrency;
using Services.Interfaces;

namespace Services
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler Default { get { return Scheduler.Default; }}
    }
}