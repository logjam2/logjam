# LogJam TODO

1. Add LogManager.Config.UseConsoleIfAvailable() - no setuplog error if not available
2. Add Log info for LogJam.Owin
  * SetupLog text URL
  * Log config + status (which writers, formatters, started, etc)
3. Add trace threshold json file - trace.config
3. Determine if Owin middleware should use Task.ConfigureAwait(false)
2. Config object refactor
    * Tracewriter config objects directly reference the LogWriterConfig objects, or ?? for "all logs"
    * Make logwriter config objects immutable when LogManager is started, and have logwriters reference their config objects instead of duplicating the properties.
3. Add text file logging
  * Create file failure should also report the current Windows user
4. Add other text formats - eg JSON, XML, delimited text (TSV, CSV)
5. Add useful log file headers - datetime opened, PID, entry point assembly, assembly version, CWD
6. Support extending default logs - eg username for HTTP requests, thread ID and name for tracing
1. Custom log rotator behavior - datetime changes, log size, etc
3. Add flush support, so buffering log writers can be flushed on command.
  * Support periodic flushing eg every .5s (more efficient than "always flush")
4. Make background logging multi log writer support periodic flushing, eg every 600ms by default.  Also support flushing from foreground delegated to background thread.
5. Instruments: Counters, Timers, HealthItems
1. Profile and perf test various use-cases
1. Custom log rotator behavior - datetime changes, log size, etc
4. Add property dependency injection for intra-LogWriter pipeline dependencies (eg SynchronizingLogWriter ref)
1. Consider renaming TraceWriterConfig, TraceWriter to TraceCollector or TraceConfig/TracerConfig or similar; make internal?
1. Create documentation site
1. Create ASP.NET v5 version, with Kestrel example
1. Consider renaming BackgroundMultiLogWriter to BackgroundThreadManager
1. Add ILogJamComponent, so the tree of components can be walked
  * Rewrite SafeStart() to use the new tree
  * Decide on approach for async start of children, IE BackgroundMultiLogWriter
	* Ensure that the SetupTracerFactory is the same for the whole component tree?  Or come up with an alternate
	approach that doesn't require that.
2. background logging multi log writer tests
  * Test that entries are logged immediately
5. SetupLog pruning - ensure it doesn't hold unneeded information.  Add a trace count?

2. Add status support, expose it from logmanager/tracemanager
3. Remote logging via protobuf
1. Implement Web API ITraceManager - more efficient than just implementing ITraceWriter?
1. Better tracing of individual LogWriter setup/shutdown

1. Support extending/customizing the TraceEntry used for tracing
  * Eg current thread ID/thread name
1. Look at adding source code file + log numbers as extension to tracing
1. Add support for XML and JSON config files
2. Complete current API unit tests
3. Add support for pruning the SetupLog to minimize memory use for long-running processes
4. Another code review pass of everything


# Done

## Version 1.0.4-beta
* LogJam.Owin - only register 1 OWIN handler when owin builder methods are called multiple times
* Add `StartableState` and make `IStartable` lifecycle more nuanced
* Add capability to configure the List to write to in `ListLogWriterConfig<TEntry>`
* Added `TraceManager.Config.TraceToList(IList<TraceEntry>)` and `LogManager.Config.UseList(IList<TEntry>)`
* LogJam.XUnit2: Fix `LogManager.Config.UseTestOutput(ITestOutputHelper);`
* Added `TraceManagerConfig.LogManagerConfig`; now whenever a TraceWriterConfig is added or removed the corresponding LogWriterConfig is instantly added or removed from the related LogManagerConfig.
* Changed `DefaultTraceFormatter.IncludeTimestamp` to default to `true`.
  

## Version 1.0.0-beta
* Use GitLink, include PDBs in NuGet packages, to enable source debugging
* Text log formatting refactor - added FormatWriter
** Added support for colorizing text, including colorized Console output
** Major improvements to console logging behavior
** Dramatically improved the GC load caused by text logging
* Support for configuring synchronization
** Add property dependency injection for intra-LogWriter pipeline dependencies (eg SynchronizingLogWriter ref)
* Upgrade all tests to xunit 2

## Version 0.9.0-beta
* Rename ILogWriter<tentry>
	-> IEntryWriter<tentry>
		, and IMultiLogWriter -> ILogWriter.  All log writers have 0 or more entrywriters within them.  Some LogWriters can only support one type.
* Added fluent configuration, refactored configuration approach
* Significantly improved unit test coverage, including multithreaded tests
* Added LogJam.XUnit2 library, for logging within xunit2 tests
* Support for configuring background logging
* Major refactor of LogJam.Owin, including configuration refactor, using background logging and unit testing
* Update OWIN logging, add unit tests and perf test for OWIN setup
* Background logging multi log writer tests
** Test that when aborting, everything that has been logged "makes it", even entries queued on a background thread.
* Separated API unit tests from internal unit tests
(Reason: Ensure that the API exposes needed functionality and no more)
