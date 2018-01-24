# LogJam TODO

1. Add netstandard versions of LogJam
2. ASP.NET Core logging (equivalent to LogJam.OWIN)
3. Make proxy/fanout EntryWriters self-updating when downstream entrywriters are started or stopped
3. Evidence perf tests (to guide decisions)
  * Compare Type.GetCSharpName() impls, using CodeDomProvider vs explicit
  * Synchronous file IO vs async
1. Fix: Severe error in startup log for console logging failing to start
  * Add LogManager.Config.UseConsoleIfAvailable() - no setuplog error if not available
1. Test that multiple debugger outputs configured results in a single instance
1. TextFileWriter
1. Headers and footers for text files
2. Events for creating and disposing log writers - unless this is costly, in which case just add this for log file writers
2. Merge rotating log file writers
2. HttpClient log writer (similar to HTTP server, but distinguishable)
2. LogJam.Powershell - formatter for Powershell host
3. Benchmark file writing perf
1. Add LogManager.Config.UseConsoleIfAvailable() - no setuplog error if not available
3. JSON text logwriter
3. Delimited text LogWriter
2. Add Log info for LogJam.Owin
  * SetupLog text URL
  * Log config + status (which writers, formatters, started, etc)
2. Fix "Missing XML comment" errors
3. Double-check access modifiers on exceptions - eg LogJamStartException has internal constructors. Is that useful for external LogWriter authors.
4. Text log file perf tests - eg buffer size tuning
3. Add trace threshold json file - trace.config
3. Determine if Owin middleware should use Task.ConfigureAwait(false)
4. Add `LogManager.Restarting` and `LogManager.Restarted` events - so eg HttpLoggingOwinMiddleware can change to write to the newest log writers
5. Add support for partial restart - eg remove logwriters that no longer exist, and add new log writers, leaving unchanged logwriters alone. Still raise the Restarted event if changes occurred.
2. Config object refactor
    * Tracewriter config objects directly reference the LogWriterConfig objects, or ?? for "all logs"
    * Make logwriter config objects immutable when LogManager is started, and have logwriters reference their config objects instead of duplicating the properties.
3. Add text file logging
  * Create file failure should also report the current Windows user
4. Add other text formats - eg JSON, XML, delimited text (TSV, CSV)
5. Make SetupLog not implement ITracerFactory - don't want people thinking they should use the setup log for normal tracing
5. Add useful log file headers - datetime opened, PID, entry point assembly, assembly version, CWD
6. Support extending default logs - eg username for HTTP requests, thread ID and name for tracing
1. Custom log rotator behavior - datetime changes, log size, etc
3. Add flush support, so buffering log writers can be flushed on command.
  * Support periodic flushing eg every .5s (more efficient than "always flush")
4. Make background logging multi log writer support periodic flushing, eg every 600ms by default. Also support flushing from foreground delegated to background thread.
5. Instruments: Counters, Timers, HealthItems
1. Profile and perf test various use-cases
1. Custom log rotator behavior - datetime changes, log size, etc
4. Add property dependency injection for intra-LogWriter pipeline dependencies (eg SynchronizingLogWriter ref)
3. Add flush support, so buffering log writers can be flushed on command.
  * Support periodic flushing eg every .5s (more efficient than "always flush")
4. Make background logging multi log writer support periodic flushing, eg every 20ms by default. Also support flushing from foreground delegated to background thread.
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
5. SetupLog pruning - ensure it doesn't hold unneeded information. Add a trace count?

2. Add status support, expose it from logmanager/tracemanager
3. Remote logging via protobuf
1. Implement Web API ITraceManager - more efficient than just implementing ITraceWriter?
1. Entity Framework tracing
1. Better tracing of individual LogWriter setup/shutdown

1. Support extending/customizing the TraceEntry used for tracing
  * Eg current thread ID/thread name
1. Look at adding source code file + log numbers as extension to tracing
1. Add support for XML and JSON config files
2. Complete current API unit tests
3. Add support for pruning the SetupLog to minimize memory use for long-running processes
4. Another code review pass of everything

