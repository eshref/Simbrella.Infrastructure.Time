using System;


namespace Simbrella.Infrastructure.Time
{
    public class TimeManager : ITimeManager
    {
        public DateTime Now => DateTime.Now;

        public double UnixTime => (DateTime.Now - DateTime.UnixEpoch.ToLocalTime()).TotalSeconds;


        public void Sleep(TimeSpan timeout)
        {
            System.Threading.Thread.Sleep(timeout);
        }
    }
}