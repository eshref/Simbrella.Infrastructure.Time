using System;


namespace Simbrella.Infrastructure.Time
{
    public interface ITimerFactory
    {
        ITimer Create(TimeSpan interval, bool singleInstance);
    }
}