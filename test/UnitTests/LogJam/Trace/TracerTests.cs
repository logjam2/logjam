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
	using LogJam.Trace.Switches;
	using LogJam.Writers;

	using Xunit;


	/// <summary>
	/// Verifies that <see cref="Tracer"/> behaves as expected.
	/// </summary>
	public sealed class TracerTests
	{
		/// <summary>
		/// Shows reasonable use for tracing to stdout in unit tests.
		/// </summary>
		[Fact]
		public void ShowRecommendedUnitTestTraceManager()
		{
			// Default threshold: Info
			using (var traceManager = new TraceManager(new ConsoleTraceWriter()))
			{
				var tracer = traceManager.TracerFor(this);
				tracer.Info("By default info is enabled");
				tracer.Verbose("This message won't be logged");
			}

			// Or, trace everything for the class under test, and info for everything else
			using (var traceManager = new TraceManager(new TraceWriterConfig(new ConsoleTraceWriter())
			{
				Switches =
														   {
															   { Tracer.All, new ThresholdTraceSwitch(TraceLevel.Info)},
															   { GetType().FullName, new OnOffTraceSwitch(true)}
														   }
			}))
			{
				traceManager.Start();
				var tracer = traceManager.TracerFor(this);
				tracer.Debug("Now debug is enabled for this class");
			}
		}

		/// <summary>
		/// Shows how to verify tracing for a class under test by using <see cref="ListLogWriter{TEntry}"/>
		/// </summary>
		[Fact]
		public void TracingCanBeVerifiedUsingListLogWriter()
		{
			var listWriter = new ListLogWriter<TraceEntry>();

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
		/// Shows how to verify tracing for a class under test
		/// </summary>
		[Fact]
		public void UnitTestTracingWithGlobalTraceManager()
		{
			// It can occur that the Tracer was obtained before the test starts
			Tracer tracer = TraceManager.Instance.TracerFor(this);

			// Traces sent to this list
			var listWriter = new ListLogWriter<TraceEntry>();

			// Add the list TraceWriter only for this class
			TraceManagerConfig config = TraceManager.Instance.Config;
			var listTraceConfig = new TraceWriterConfig(listWriter)
			                      {
				                      Switches =
				                      {
					                      { GetType().FullName, new OnOffTraceSwitch(true) }
				                      }
			                      };
			config.Writers.Add(listTraceConfig);
			// restart to load config and assign writers
			TraceManager.Instance.Start();

			tracer.Info("Info message");
			tracer.Debug("Debug message");
			Assert.Equal(2, listWriter.Count);

			// Remove the TraceWriterConfig just to ensure that everything returns to normal
			Assert.True(TraceManager.Instance.Config.Writers.Remove(listTraceConfig));
			// restart to reset config
			TraceManager.Instance.Start();

			TraceManagerConfigTests.AssertEquivalentToDefaultTraceManagerConfig(TraceManager.Instance);

			// Now tracing goes to the debug window only, but not to the list
			tracer.Info("Not logged to list, but logged to debug out.");
			Assert.Equal(2, listWriter.Count);
		}

		[Fact]
		public void LogWriterExceptionsDontPropagate()
		{
			var exceptionLogWriter = new ExceptionThrowingLogWriter<TraceEntry>();
			var listLogWriter = new ListLogWriter<TraceEntry>();
			var traceManagerConfig = new TraceManagerConfig(
				new TraceWriterConfig(exceptionLogWriter)
				{
					Switches =
					{
						{ Tracer.All, new OnOffTraceSwitch(true)}
					}
				},
				new TraceWriterConfig(listLogWriter)
				{
					Switches =
					{
						{ Tracer.All, new OnOffTraceSwitch(true)}
					}
				});

			var traceManager = new TraceManager(traceManagerConfig);
			using (traceManager)
			{
				traceManager.Start();

				var tracer = traceManager.TracerFor(this);
				tracer.Info("Info");
				tracer.Warn("Warn");

				Assert.Equal(2, listLogWriter.Count());
				Assert.Equal(2, exceptionLogWriter.CountExceptionsThrown);

				// First write exception is reported in SetupTraces
				// TODO: Replace logging to SetupTraces with TraceWriter reporting its current status
				Assert.Equal(1, traceManager.SetupTraces.Count(traceEntry => traceEntry.TraceLevel >= TraceLevel.Error && traceEntry.Details != null));
			}

			Assert.Equal(3, exceptionLogWriter.CountExceptionsThrown);

			// Exceptions should be reported in the SetupTraces
			Assert.Equal(2, traceManager.SetupTraces.Count(traceEntry => traceEntry.TraceLevel >= TraceLevel.Error && traceEntry.Details != null));
		}


		private class ExceptionThrowingLogWriter<TEntry> : ILogWriter<TEntry>, IDisposable where TEntry : ILogEntry
		{

			public int CountExceptionsThrown = 0;

			public bool Enabled { get { return true; } }
			public bool IsSynchronized { get { return true; } }

			public void Write(ref TEntry entry)
			{
				CountExceptionsThrown++;
				throw new ApplicationException();
			}

			public void Dispose()
			{
				CountExceptionsThrown++;
				throw new ApplicationException();
			}

		}
	}

}