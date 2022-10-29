# Introduction 
Simple abstraction over DateTime.Now

# Getting Started
In order to work with ITimeManager in you net core projects add below line to `Startup.ConfigureServices` method.
`services.AddSingleton<ITimeManager, TimeManager>();`
