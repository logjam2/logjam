// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TraceManagerTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Trace
{
    using System.Linq;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Trace.Config;
    using LogJam.UnitTests.Examples;
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

            using (var traceManager = new TraceManager(logManager, TraceManagerConfig.Default()))
            {
                traceManager.Start();

                // Starting the TraceManager starts the LogManager
                Assert.True(logManager.IsStarted);

                // There should be two started LogWriters - one is the DebuggerLogWriter for tracing; the other is messageListWriter
                Assert.Equal(2, logManager.Config.Writers.Where(writerConfig => ((IStartable) logManager.GetLogWriter(writerConfig)).IsStarted).Count());

                Assert.True(logManager.IsHealthy); // Ensure no warnings or errors
            }
        }

    }

}
