// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="TracerTests.cs">
// Copyright (c) 2011-2014 logjam.codeplex.com.  
// </copyright>
// Licensed under the <a href="http://logjam.codeplex.com/license">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace
{
	using System;
	using System.Linq;

	using LogJam.Config;
	using LogJam.Trace;
	using LogJam.Trace.Config;
	using LogJam.Trace.Format;
	using LogJam.Trace.Switches;
	using LogJam.UnitTests.Common;
	using LogJam.UnitTests.Examples;
	using LogJam.Writer;

	using Xunit;
	using Xunit.Extensions;


	/// <summary>
	/// Verifies that <see cref="Tracer"/> behaves as expected.
	/// </summary>
	public sealed class TracerTests
	{

		/// <summary>
		/// Shows reasonable use for tracing to stdout in unit tests.
		/// </summary>
		[Fact]
		public void BasicConsoleTracing()
		{
			// Default threshold: Info
			using (var traceManager = new TraceManager(new ConsoleLogWriterConfig().Format(new DefaultTraceFormatter())))
			{
				var tracer = traceManager.TracerFor(this);

				Assert.True(traceManager.IsStarted); // Getting a tracer starts the TraceManager
				Assert.True(tracer.IsInfoEnabled());
				Assert.False(tracer.IsVerboseEnabled());

				tracer.Info("By default info is enabled");
				tracer.Verbose("This message won't be logged");
			}

			// Or, trace everything for the class under test, and info for everything else
			var config = new TraceWriterConfig(new ConsoleLogWriterConfig().Format(new DefaultTraceFormatter()))
			             {
				             Switches =
				             {
					             { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
					             { GetType(), new OnOffTraceSwitch(true) }
				             }
			             };
			using (var traceManager = new TraceManager(config))
			{
				var tracer = traceManager.TracerFor(this);
				tracer.Debug("Now debug is enabled for this class");

				Assert.True(tracer.IsVerboseEnabled());
				Assert.True(tracer.IsDebugEnabled());
			}
		}

		[Fact]
		public void BasicFluentConsoleTracing()
		{
			var traceConfig = new TraceManagerConfig();
			traceConfig.TraceToConsole();
			using (var traceManager = new TraceManager(traceConfig))
			{
				var tracer = traceManager.TracerFor(this);

				Assert.True(traceManager.IsStarted); // Getting a tracer starts the TraceManager
				Assert.True(tracer.IsInfoEnabled());
				Assert.False(tracer.IsVerboseEnabled());

				tracer.Info("By default info is enabled");
				tracer.Verbose("This message won't be logged");
			}

			// Or, trace everything for the class under test, and info for everything else
			traceConfig = new TraceManagerConfig();
			traceConfig.TraceToConsole(new SwitchSet()
			                           {
				                           { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info) },
				                           { GetType(), new OnOffTraceSwitch(true) }
			                           });
			using (var traceManager = new TraceManager(traceConfig))
			{
				var tracer = traceManager.TracerFor(this);
				tracer.Debug("Now debug is enabled for this class");

				Assert.True(tracer.IsVerboseEnabled());
				Assert.True(tracer.IsDebugEnabled());
			}
		}

		/// <summary>
		/// Shows how to verify tracing for a class under test by using <see cref="ListLogWriter{TEntry}"/>
		/// </summary>
		[Fact]
		public void TracingCanBeVerifiedUsingListLogWriter()
		{
			var setupTracerFactory = new SetupLog();
			var listWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

			// Default threshold: Info for everything
			Tracer tracer;
			using (var traceManager = new TraceManager(listWriter))
			{
				tracer = traceManager.TracerFor(this);
				tracer.Info("By default info is enabled");
				tracer.Debug("This message won't be logged");
			}
			Assert.Single(listWriter);
			listWriter.Clear();

			// Tracing after TraceManager is disposed does nothing
			tracer.Info("Not logged, b/c TraceManager has been disposed.");
			Assert.Empty(listWriter);

			// Trace everything for the test class, nothing for other types
			using (var traceManager = new TraceManager(listWriter, new OnOffTraceSwitch(true), GetType().FullName))
			{
				tracer = traceManager.TracerFor(this);
				tracer.Debug("Now debug is enabled for this class");
			}
			Assert.Single(listWriter);
		}

		/// <summary>
		/// Shows how to verify tracing for a class under test; and how to re-use the global <see cref="TraceManager.Instance"/>.
		/// </summary>
		[Theory]
		[InlineData(ConfigForm.Fluent)]
		[InlineData(ConfigForm.ObjectGraph)]
		[InlineData(ConfigForm.Fluent)]
		[InlineData(ConfigForm.ObjectGraph)]
		public void UnitTestTracingWithGlobalTraceManager(ConfigForm configForm)
		{
			// In a real app you probably don't have to reset the LogManager + TraceManager before configuring
			LogManager.Instance.Reset(true);
			TraceManager.Instance.Reset(true);
			Assert.Same(LogManager.Instance, TraceManager.Instance.LogManager);
			Assert.Empty(LogManager.Instance.Config.Writers);
			Assert.Empty(TraceManager.Instance.Config.Writers);
			Assert.True(TraceManager.Instance.IsHealthy);

			// Add the default Tracer config (traces to debugger)
			TraceManager.Instance.Config.TraceToDebugger();

			// It can occur that the Tracer was obtained before the test starts
			Tracer tracer = TraceManager.Instance.TracerFor(this);

			// Traces sent to this list
			var setupTracerFactory = TraceManager.Instance.SetupTracerFactory;
			var listWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);

			// Add the list TraceWriter only for this class
			TraceManagerConfig config = TraceManager.Instance.Config;
			TraceWriterConfig listTraceConfig;
			if (configForm == ConfigForm.ObjectGraph)
			{
				listTraceConfig = new TraceWriterConfig(listWriter)
				                      {
					                      Switches =
					                      {
						                      { GetType(), new OnOffTraceSwitch(true) }
					                      }
				                      };
				config.Writers.Add(listTraceConfig);
			}
			else if (configForm == ConfigForm.Fluent)
			{
				listTraceConfig = config.UseLogWriter(listWriter, GetType(), new OnOffTraceSwitch(true));
			}
			else
			{
				throw new NotImplementedException();
			}

			// restart to load config and assign writers
			TraceManager.Instance.Start();

			// Ensure start didn't result in any errors
			Assert.True(TraceManager.Instance.IsHealthy);

			tracer.Info("Info message");
			tracer.Debug("Debug message");
			Assert.Equal(2, listWriter.Count);

			// Remove the TraceWriterConfig just to ensure that everything returns to normal
			Assert.True(TraceManager.Instance.Config.Writers.Remove(listTraceConfig));
			// restart to reset config
			TraceManager.Instance.Start();

			LogJam.Internal.UnitTests.Trace.TraceManagerConfigTests.AssertEquivalentToDefaultTraceManagerConfig(TraceManager.Instance);

			// Now tracing goes to the debug window only, but not to the list
			tracer.Info("Not logged to list, but logged to debug out.");
			Assert.Equal(2, listWriter.Count);
		}

		[Fact]
		public void LogWriterExceptionsDontPropagate()
		{
			var setupTracerFactory = new SetupLog();
			var exceptionLogWriter = new ExceptionThrowingLogWriter<TraceEntry>(setupTracerFactory);
			var listLogWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);
			var traceManagerConfig = new TraceManagerConfig(
				new TraceWriterConfig()
				{
					LogWriterConfig = new UseExistingLogWriterConfig(exceptionLogWriter, disposeOnStop: true),
					Switches =
					{
						{ Tracer.All, new OnOffTraceSwitch(true)}
					}
				},
				new TraceWriterConfig(listLogWriter)
				{
					LogWriterConfig = new UseExistingLogWriterConfig(listLogWriter, disposeOnStop: true),
					Switches =
					{
						{ Tracer.All, new OnOffTraceSwitch(true)}
					}
				});

			var traceManager = new TraceManager(traceManagerConfig, setupTracerFactory);
			using (traceManager)
			{
				traceManager.Start();

				var tracer = traceManager.TracerFor(this);
				tracer.Info("Info");
				tracer.Warn("Warn");

				Assert.Equal(2, listLogWriter.Count());
				Assert.Equal(2, exceptionLogWriter.CountExceptionsThrown);

				// First write exception is reported in SetupLog
				// TODO: Replace logging to SetupLog with TraceWriter reporting its current status
				Assert.Equal(1, traceManager.SetupLog.Count(traceEntry => traceEntry.TraceLevel >= TraceLevel.Error && traceEntry.Details != null));
			}

			Assert.Equal(3, exceptionLogWriter.CountExceptionsThrown);

			// Exceptions should be reported in the SetupLog
			Assert.Equal(2, traceManager.SetupLog.Count(traceEntry => traceEntry.TraceLevel >= TraceLevel.Error && traceEntry.Details != null));
		}

		/// <summary>
		/// Ensures that TracerNames for generic types are consistent and readable
		/// </summary>
		[Fact]
		public void TracerNamesForGenericTypes()
		{
			var tracerFactory = new SetupLog();

			// ListLogWriter<MessageEntry> handling (generic type parameter)
			var tracer1 = tracerFactory.GetTracer(typeof(ListLogWriter<MessageEntry>));
			Console.WriteLine(tracer1.Name);
			var tracer2 = tracerFactory.GetTracer(typeof(ListLogWriter<>));
			Console.WriteLine(tracer2.Name);
			var tracer3 = tracerFactory.TracerFor(new ListLogWriter<MessageEntry>(new SetupLog()));
			Console.WriteLine(tracer3.Name);
			var tracer4 = tracerFactory.TracerFor<ListLogWriter<MessageEntry>>();
			Console.WriteLine(tracer4.Name);
			var tracer5 = tracerFactory.GetTracer(typeof(ListLogWriter<MessageEntry>).GetGenericTypeDefinition());
			Console.WriteLine(tracer5.Name);

			// PrivateClass.TestLogWriter<MessageEntry> handling (inner class + generic type parameter)
			tracer1 = tracerFactory.GetTracer(typeof(PrivateClass.TestEntryWriter<MessageEntry>));
			Console.WriteLine(tracer1.Name);
			tracer2 = tracerFactory.GetTracer(typeof(PrivateClass.TestEntryWriter<>));
			Console.WriteLine(tracer2.Name);
			tracer3 = tracerFactory.TracerFor(new PrivateClass.TestEntryWriter<MessageEntry>());
			Console.WriteLine(tracer3.Name);
			tracer4 = tracerFactory.TracerFor<PrivateClass.TestEntryWriter<MessageEntry>>();
			Console.WriteLine(tracer4.Name);
			tracer5 = tracerFactory.GetTracer(typeof(PrivateClass.TestEntryWriter<MessageEntry>).GetGenericTypeDefinition());
			Console.WriteLine(tracer5.Name);
		}

		[Fact]
		public void EnableTracingForAllGenericTypesWithSameGenericTypeDefinition()
		{
			var setupTracerFactory = new SetupLog();
			var traceConfig = new TraceWriterConfig(new ListLogWriter<TraceEntry>(setupTracerFactory))
			                  {
				                  Switches =
				                  {
					                  { typeof(PrivateClass.TestEntryWriter<>), new OnOffTraceSwitch(true) }
				                  }
			                  };

			using (var traceManager = new TraceManager(traceConfig, setupTracerFactory))
			{
				var tracer = traceManager.TracerFor(this);
				Assert.False(tracer.IsInfoEnabled());

				tracer = traceManager.GetTracer(typeof(PrivateClass.TestEntryWriter<>));
				Assert.True(tracer.IsInfoEnabled());

				tracer = traceManager.TracerFor<PrivateClass.TestEntryWriter<MessageEntry>>();
				Assert.True(tracer.IsInfoEnabled());

				tracer = traceManager.TracerFor<PrivateClass.TestEntryWriter<TraceEntry>>();
				Assert.True(tracer.IsInfoEnabled());
			}
		}

		[Fact]
		public void EnableTracingForSpecificGenericType()
		{
			var setupTracerFactory = new SetupLog();
			var traceConfig = new TraceWriterConfig(new ListLogWriter<TraceEntry>(setupTracerFactory))
			                  {
				                  Switches =
				                  {
					                  { typeof(PrivateClass.TestEntryWriter<MessageEntry>), new OnOffTraceSwitch(true) }
				                  }
			                  };
			using (var traceManager = new TraceManager(traceConfig, setupTracerFactory))
			{
				var tracer = traceManager.TracerFor(this);
				Assert.False(tracer.IsInfoEnabled());

				tracer = traceManager.GetTracer(typeof(PrivateClass.TestEntryWriter<>));
				Assert.False(tracer.IsInfoEnabled());

				tracer = traceManager.TracerFor<PrivateClass.TestEntryWriter<MessageEntry>>();
				Assert.True(tracer.IsInfoEnabled());

				tracer = traceManager.TracerFor<PrivateClass.TestEntryWriter<TraceEntry>>();
				Assert.False(tracer.IsInfoEnabled());
			}
		}

		/// <summary>
		/// Used just to test <see cref="Tracer"/> naming in <see cref="TracerTests.TracerNamesForGenericTypes"/>
		/// </summary>
// ReSharper disable once ClassNeverInstantiated.Local
		private class PrivateClass
		{

			internal class TestEntryWriter<TEntry> : IEntryWriter<TEntry>
				where TEntry : ILogEntry
			{

				public void Write(ref TEntry entry)
				{
					throw new NotImplementedException();
				}

				public bool IsEnabled { get { return true; } }
				public Type LogEntryType { get { return typeof(TEntry); } }

			}

		}
	}

}