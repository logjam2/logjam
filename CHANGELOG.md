# ChangeLog for LogJam projects 

## Version 1.0.5
* #17 Add support for switching the ITestOutputHelper per test, without restarting the LogManager

## Version 1.0.4
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
		, and IMultiLogWriter -> ILogWriter. All log writers have 0 or more entrywriters within them. Some LogWriters can only support one type.
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
