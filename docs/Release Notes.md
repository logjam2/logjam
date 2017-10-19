# LogJam Release Notes

## Version 1.1.0
* [Added: netstandard 1.3 versions of LogJam and LogJam.XUnit2](https://github.com/logjam2/logjam/issues/21)
  * Code Contracts files are not included in netstandard platform directory; Code Contracts files are included in `net45` directory. `net45` DLLs use Windows PDBs; netstandard DLLs use Portable PDBs, and Code Contracts' ccrewrite fails for portable PDBs.
  * All tests are run on the following platforms: netcoreapp2.0;net452;net47
* Added `TraceManagerConfig.TypeNameFunc` - can be used to customize how the `Tracer.Name` is determined for a type.
* Builds are now public and run on appveyor: https://ci.appveyor.com/project/johncrim/logjam
* New netstandard libraries [use SourceLink](https://github.com/ctaggart/SourceLink) to support downloading source code from github while debugging

## Version 1.0.6
* [Fixed: LogJam.Owin exception on 404 pages when running under IIS](https://github.com/logjam2/logjam/issues/22)

## Version 1.0.5
* [Added: Support for support for changing the xunit ITestOutputHelper per test](https://github.com/logjam2/logjam/issues/17)

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
