using System;
using System.Threading;
using System.Timers;


namespace Simbrella.Infrastructure.Time
{
    public class SafeTimer : ITimer
    {
        private System.Timers.Timer _internalTimer;
        private readonly object _locker;

        private bool _isStarted;
        private readonly object _startStopLocker = new object();
        private ManualResetEventSlim _stopWaitHandle;

        private long _activeInstances;
        private readonly object _activeInstancesLocker = new object();

        private readonly object _state;


        public SafeTimer(TimeSpan interval)
            : this(interval, false)
        { }

        public SafeTimer(TimeSpan interval, bool singleInstance)
            : this(interval, singleInstance, null)
        { }

        public SafeTimer(TimeSpan interval, bool singleInstance, object state)
        {
            if (singleInstance)
                _locker = new object();

            _internalTimer = new System.Timers.Timer(interval.TotalMilliseconds);
            _internalTimer.AutoReset = true;

            _state = state;
        }


        public void Dispose()
        {
            if (this.Disposed)
                return;

            this.Disposed = true;

            lock (_startStopLocker)
            {
                if (_isStarted)
                    this.internalStop(false);

                _internalTimer.Dispose();
                _internalTimer = null;
            }
        }


        public bool Disposed { get; private set; }

        public event EventHandler<TimerElapsedEventArgs> Elapsed;


        public void Start()
        {
            if (this.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_startStopLocker)
            {
                if (_isStarted)
                    return;

                this.internalStart();
            }
        }

        public void Stop(bool waitAllInstances)
        {
            if (this.Disposed)
                throw new ObjectDisposedException(this.GetType().Name);

            lock (_startStopLocker)
            {
                if (!_isStarted)
                    return;

                this.internalStop(waitAllInstances);
            }
        }

        public void TriggerElapsed()
        {
            this.internalTimerInternalElapsed(DateTime.Now);
        }


        private void internalStart()
        {
            _isStarted = true;

            _stopWaitHandle = new ManualResetEventSlim();

            _internalTimer.Elapsed += this.internalTimerElapsed;

            _internalTimer.Start();
        }

        private void internalStop(bool waitAllInstances)
        {
            _isStarted = false;

            if (waitAllInstances && Volatile.Read(ref _activeInstances) > 0)
            {
                _stopWaitHandle.Wait();
            }

            _stopWaitHandle.Dispose();
            _stopWaitHandle = null;

            _internalTimer.Elapsed -= this.internalTimerElapsed;
        }

        private void internalTimerInternalElapsed(DateTime signalTime)
        {
            if (!_isStarted)
                return;

            _internalTimer.Start();

            bool lockTaken = false;

            if (_locker != null)
            {
                Monitor.TryEnter(_locker, ref lockTaken);

                if (!lockTaken)
                    return;
            }

            try
            {
                this.onElapsed(signalTime);
            }
            finally
            {
                if (_locker != null && lockTaken)
                    Monitor.Exit(_locker);
            }
        }

        private void internalTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.internalTimerInternalElapsed(e.SignalTime);
        }

        private void onElapsed(DateTime signalTime)
        {
            var stopWaitHandle = _stopWaitHandle;

            lock (_activeInstancesLocker)
                _activeInstances++;

            try
            {
                this.Elapsed?.Invoke(this, new TimerElapsedEventArgs(signalTime, _state));
            }
            finally
            {
                lock (_activeInstancesLocker)
                {
                    _activeInstances--;

                    if (!_isStarted && stopWaitHandle != null && _activeInstances == 0)
                    {
                        try
                        {
                            stopWaitHandle.Set();
                        }
                        catch (ObjectDisposedException) { }
                    }
                }
            }
        }
    }
}