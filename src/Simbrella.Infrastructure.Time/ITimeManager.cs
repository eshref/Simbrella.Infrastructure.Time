using System;


namespace Simbrella.Infrastructure.Time
{
    public interface ITimeManager
    {
        DateTime Now { get; }

        double UnixTime { get; }

        void Sleep(TimeSpan timeout);
    }
}