using System;


namespace Simbrella.Infrastructure.Time
{
    public class TimerElapsedEventArgs : EventArgs
    {
        public new static readonly TimerElapsedEventArgs Empty = new TimerElapsedEventArgs();

        private TimerElapsedEventArgs() { }


        public TimerElapsedEventArgs(DateTime signalTime, object state)
        {
            this.SignalTime = signalTime;
            this.State = state;
        }


        public DateTime SignalTime { get; }
        public object State { get; }
    }
}