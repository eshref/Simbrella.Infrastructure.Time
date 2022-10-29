using System;


namespace Simbrella.Infrastructure.Time
{
    public interface ITimer : IDisposable
    {
        void Start();
        void Stop(bool waitAllInstances);

        event EventHandler<TimerElapsedEventArgs> Elapsed;
    }
}