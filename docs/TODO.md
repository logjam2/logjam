# LogJam TODO

1. Add serializing multi log writer
    (eg can serialize across multiple log writers/formatters)
2. Add background logging multi log writer
    Test that entries are logged immediately
	  Test that when aborting, everything that has been logged "makes it"
1. Update OWIN logging, add unit tests for OWIN setup
1. Look at adding file + log numbers as extension to tracing
1. Add support for XML and JSON config files
1. Better tracing of individual LogWriter setup/shutdown
1. Separate API unit tests from internal unit tests
(Reason: Ensure that the API exposes needed functionality and no more)
2. Complete current API unit tests
2. Add status support, expose it from logmanager/tracemanager
3. Remote logging via protobuf
