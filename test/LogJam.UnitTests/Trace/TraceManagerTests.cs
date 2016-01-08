// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace
{
    using System;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Test.Shared;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.Trace.Switches;
    using LogJam.Writer;

    using Xunit;


    /// <summary>
    /// Unit tests for <see cref="TraceManager" />
    /// </summary>
    public sealed class TraceManagerTests
    {

        [Fact]
        public void EachTraceManagerHasALogManager()
        {
            using (var traceManager = new TraceManager())
            {
                var logManager = traceManager.LogManager;
                Assert.NotNull(logManager);
                Assert.Equal(traceManager.IsStarted, logManager.IsStarted);

                traceManager.Start();
                Assert.True(logManager.IsStarted);

                traceManager.Stop();
                Assert.False(logManager.IsStarted);
            }
        }

        [Fact]
        public void GettingATracerStartsTheTraceManagerAndLogManager()
        {
            TraceManager traceManager;
            using (traceManager = new TraceManager())
            {
                Assert.False(traceManager.IsStarted);
                Assert.False(traceManager.LogManager.IsStarted);

                var tracer = traceManager.TracerFor(this);

                Assert.True(traceManager.IsStarted);
                Assert.True(traceManager.LogManager.IsStarted);
            }

            Assert.False(traceManager.IsStarted);
            Assert.False(traceManager.LogManager.IsStarted);
        }

        [Fact]
        public void LogManagerCanBePassedToTraceManagerCtor()
        {
            var setupLog = new SetupLog();
            var logConfig = new LogManagerConfig();

            // Just a logwriter that is not used for tracing
            var messageListWriter = new ListLogWriter<MessageEntry>(setupLog);
            var messageListWriterConfig = logConfig.UseLogWriter(messageListWriter);
            var logManager = new LogManager(logConfig, setupLog);

            // The TraceManagerConfig includes a DebugTraceWriter, which adds to the LogManager.Writers
            using (var traceManager = new TraceManager(logManager, new TraceManagerConfig(TraceManagerConfig.CreateDebugTraceWriterConfig())))
            {
                traceManager.Start();

                // Starting the TraceManager starts the LogManager
                Assert.True(logManager.IsStarted);

                // There should be two started LogWriters - one is the DebuggerLogWriter for tracing; the other is messageListWriter
                Assert.Equal(2, logManager.Config.Writers.Where(writerConfig => ((IStartable) logManager.GetLogWriter(writerConfig)).IsStarted).Count());

                Assert.True(logManager.IsHealthy); // Ensure no warnings or errors
            }
        }

        [Fact]
        public void RootLogWriterCanBeSetOnInitialization()
        {
            var setupTracerFactory = new SetupLog();
            // Trace output is written to this guy
            var listLogWriter = new ListLogWriter<TraceEntry>(setupTracerFactory);
            using (var traceManager = new TraceManager(listLogWriter))
            {
                traceManager.Start();
                var tracer = traceManager.TracerFor(this);
                tracer.Info("Info");

                Assert.Single(listLogWriter);
                TraceEntry traceEntry = listLogWriter.First();
                Assert.Equal(GetType().GetCSharpName(), traceEntry.TracerName);
                Assert.Equal(TraceLevel.Info, traceEntry.TraceLevel);
            }
        }

        /// <summary>
        /// Shows how to verify tracing for a class under test; and how to re-use the global <see cref="TraceManager.Instance" />.
        /// </summary>
        /// <remarks>
        /// Using these global instances is not preferred for unit testing, but is workable, particularly if the trace log
        /// configuration is
        /// the same for all tests. The main issue with using global instances is that tests run in parallel will all use the same
        /// instances.
        /// </remarks>
        [Theory]
        [InlineData(ConfigForm.Fluent, 1)]
        [InlineData(ConfigForm.ObjectGraph, 2)]
        [InlineData(ConfigForm.Fluent, 3)]
        [InlineData(ConfigForm.ObjectGraph, 4)]
        public void UnitTestTracingWithGlobalTraceManager(ConfigForm configForm, int iteration)
        {
            // It can occur that the Tracer was obtained before the test starts
            Tracer tracer = TraceManager.Instance.TracerFor(this);

            // In a real app you probably don't have to reset the LogManager + TraceManager before configuring.
            // However for unit testing it can be a good idea to clear configuration from previous tests.
            LogManager.Instance.Reset(true);
            TraceManager.Instance.Reset(true);
            Assert.Same(LogManager.Instance, TraceManager.Instance.LogManager);

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

    }

}
