using System;


namespace Simbrella.Infrastructure.Time
{
    public class SafeTimerFactory : ITimerFactory
    {
        public ITimer Create(TimeSpan interval, bool singleInstance)
        {
            return new SafeTimer(interval, singleInstance);
        }
    }
}