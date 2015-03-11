# LogJam TODO

1. Update OWIN logging, add unit tests and perf test for OWIN setup
1. Rename ILogWriter<TEntry> -> IEntryWriter<TEntry>, and IMultiLogWriter -> ILogWriter.  All log writers have 0 or more entrywriters within them.  Some LogWriters can only support one type.
1. Add LogJamComponent, so the tree of components can be walked
  * Rewrite SafeStart() to use the new tree
  * Decide on approach for async start of children, IE BackgroundMultiLogWriter
2. background logging multi log writer tests
  * Test that entries are logged immediately
	* Test that when aborting, everything that has been logged "makes it"
3. Add flush support, so buffering log writers can be flushed on command.
  * Support periodic flushing eg every .5s (more efficient than "always flush")
4. Make background logging multi log writer support periodic flushing, eg every 20ms by default.  Also support flushing from foreground delegated to background thread.
5. Add colorizing log formatter, colorized consolelogwriter

1. Add LogJam.XUnit2 - with logwriter and formatter for unit tests
2. Add status support, expose it from logmanager/tracemanager
3. Remote logging via protobuf
1. Custom log rotator behavior - datetime changes, log size, etc
1. Better tracing of individual LogWriter setup/shutdown
1. Support extending/customizing the TraceEntry used for tracing
  * Eg current thread ID/thread name
1. Consider removing the *Config suffix from config types for friendlier configuration; and/or evaluate fluent config.
1. Look at adding source code file + log numbers as extension to tracing
1. Add support for XML and JSON config files
1. Separate API unit tests from internal unit tests
(Reason: Ensure that the API exposes needed functionality and no more)
2. Complete current API unit tests
