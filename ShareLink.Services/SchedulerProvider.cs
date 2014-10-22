
using Services.Interfaces;
using System.Reactive.Concurrency;

namespace Services
{
    public class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler Default { get { return Scheduler.Default; }}
    }
}