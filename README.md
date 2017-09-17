
*This project was previously hosted at [https://logjam.codeplex.com/](https://logjam.codeplex.com/)*

# What is LogJam?
LogJam is a modern, efficient, and productive system for trace logging and metrics logging for all projects.

LogJam is currently in beta and is initially focused on the needs of C# and web developers.

LogJam packages are available via NuGet: [http://www.nuget.org/packages?q=logjam](http://www.nuget.org/packages?q=logjam)

The main LogJam library targets .NET 4.0 and later, though LogJam integrations may use later versions based on the requirements of the project that is integrated with.

<a href="https://ci.appveyor.com/api/projects/status/github/logjam2/logjam">
<img src="https://ci.appveyor.com/api/projects/status/github/logjam2/logjam?svg=true" alt="Appveyor build status" width="150" style="margin:1em"></a>

# Current Status
LogJam is productive and useful for these types of projects:

* .NET libraries
* .NET Command-line applications
* .NET Web applications using OWIN and/or Web API
* XUnit 2.0 tests

Its features include:

* Productive and efficient log API for logging arbitrary schemas (aka metrics logging)
  * Multiple log schemas can be combined in a single target
* Productive API for trace logging
  * Trace switches can be replaced with new implementations
* Configuration in code
  * Configuration can be updated without restarting the application
* Logging to binary or text targets
  * Logging to text uses replaceable formatters
* Threadsafe log writing on a background thread - improves performance for logging clients.
* All internal setup operations are traced to a setup log
* Best-of-breed efficiency - including minimizing locks and impact on the managed heap

Current API integrations:

* OWIN HTTP and trace logging
* Web API trace logging
* XUnit 2.0 test output

Current log targets:

* Console
* Any ```TextWriter``` - including files
* .NET debugger
* XUnit 2.0 test output
* ```List<TLogEntry>``` - in memory collection of log entries.

## Near-term Goals
* Add instrumentation classes
* LogJam service providing centralized network logging
  * Browse and query logs via web service
* XML and JSON configuration files
* Mono support

# Longer-term Goals
LogJam intends to be a multi-project framework that provides best-of-breed logging APIs and functionality for all project types - library projects, command-line applications, web applications, web services, GUI applications, and phone applications. Additional features will include:

* Text and binary log files that can be read and queried via LINQ - in other word, persisted logs are round-trippable
* In-memory log queues (trailing window of log data is held in memory)
* 2-way interoperability with .NET Framework logging - projects that use System.Diagnostics logging can write to LogJam.Trace; and projects that use LogJam.Trace can write to System.Diagnostics TraceListeners.
* Support central management of logging configuration - across many servers and applications
* Web UI for managing configuration and browsing/querying logs across server farms and applications
* Optional integration with ETW (Event Tracing for Windows)
