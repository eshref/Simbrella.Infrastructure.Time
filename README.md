# Introduction 
Simple abstraction over `DateTime.Now` and `System.Timers.Timer` with `SafeTimer` implementation.

# Usage
In order to work with `ITimeManager` in you net core projects add below line to `Startup.ConfigureServices` method.
`services.AddSingleton<ITimeManager, TimeManager>();`

In order to work with `ITimer` in you net core projects add below line to `Startup.ConfigureServices` method.
`services.AddSingleton<ITimerFactory, SafeTimerFactory>();`

Then inject it to your dependent classes as below.

```
using Microsoft.Extensions.Logging;

using Simbrella.Infrastructure.Time;

namespace ClassLibrary1
{
    public class SomeClass : IDisposable
    {
        private readonly ITimer _timer;
        
        private readonly ILogger _logger;

        private bool _isStarted;


        public SomeClass(
            ITimerFactory timerFactory, TimerOptions timerOptions, ILogger logger)
        {
            if (timerFactory == null) throw new ArgumentNullException(nameof(timerFactory));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _timer = timerFactory.Create(timerOptions.Interval, timerOptions.SingleInstance);
        }


        public void Dispose()
        {
            if (this.Disposed)
                return;

            this.Disposed = true;

            if (_isStarted)
                this.internalStop();

            _timer.Dispose();
        }


        public bool Disposed { get; private set; }


        public void Start()
        {
            if (this.Disposed) throw new ObjectDisposedException(this.GetType().Name);

            if (_isStarted)
                return;

            this.internalStart();
        }

        public void Stop()
        {
            if (this.Disposed) throw new ObjectDisposedException(this.GetType().Name);

            if (!_isStarted)
                return;

            this.internalStop();
        }


        private void internalStart()
        {
            _isStarted = true;

            _timer.Elapsed += this.timerElapsed;
            _timer.Start();
        }

        private void internalStop()
        {
            _isStarted = false;

            _timer.Stop(false);
            _timer.Elapsed -= this.timerElapsed;
        }

        private void timerElapsed(object sender, TimerElapsedEventArgs e)
        {
            _logger.LogInformation("timer interval elapsed.");
        }
    }
}
```
