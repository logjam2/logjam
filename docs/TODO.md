# LogJam TODO

1. Support for configuring synchronization
1. Consider renaming TraceWriterConfig, TraceWriter to TraceCollector or TraceConfig/TracerConfig or similar; make internal?
1. Consider renaming BackgroundMultiLogWriter to BackgroundThreadManager
1. Add ILogJamComponent, so the tree of components can be walked
  * Rewrite SafeStart() to use the new tree
  * Decide on approach for async start of children, IE BackgroundMultiLogWriter
	* Ensure that the SetupTracerFactory is the same for the whole component tree?  Or come up with an alternate
	approach that doesn't require that.
2. background logging multi log writer tests
  * Test that entries are logged immediately
3. Add flush support, so buffering log writers can be flushed on command.
  * Support periodic flushing eg every .5s (more efficient than "always flush")
4. Make background logging multi log writer support periodic flushing, eg every 20ms by default.  Also support flushing from foreground delegated to background thread.
5. Add colorizing log formatter, colorized consolelogwriter

2. Add status support, expose it from logmanager/tracemanager
3. Remote logging via protobuf
1. Custom log rotator behavior - datetime changes, log size, etc
1. Better tracing of individual LogWriter setup/shutdown

1. Support extending/customizing the TraceEntry used for tracing
  * Eg current thread ID/thread name
1. Look at adding source code file + log numbers as extension to tracing
1. Add support for XML and JSON config files
2. Complete current API unit tests


# Done

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
