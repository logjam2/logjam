// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Trace;
    using LogJam.UnitTests.Common;
    using LogJam.UnitTests.Examples;

    using Xunit;


    /// <summary>
    /// Unit tests for <see cref="LogManager" />.
    /// </summary>
    public sealed class LogManagerTests
    {

        [Fact]
        public void DefaultLogManagerTracksAllLogJamOperationsToStatusTraces()
        {
            string testMessage = "test LogJam setup message";
            var traceLevel = TraceLevel.Info;

            using (var logManager = new LogManager())
            {
                // Trace a message
                var internalTracer = logManager.SetupTracerFactory.TracerFor(this);
                internalTracer.Trace(traceLevel, null, testMessage);

                // Verify the message can be found in LogJamTraceEntries
                var traceEntry = logManager.SetupLog.First(e => e.Message == testMessage);
                Assert.Equal(traceLevel, traceEntry.TraceLevel);
                Assert.Equal(internalTracer.Name, traceEntry.TracerName);
                int countTraces1 = logManager.SetupLog.Count();

                // Messing with the SetupLog doesn't force a Start()
                Assert.False(logManager.IsStarted);

                // Start and stop create trace records
                logManager.Start();
                int countTracesAfterStart = logManager.SetupLog.Count();
                Assert.True(countTracesAfterStart > countTraces1);

                logManager.Stop();
                int countTracesAfterStop = logManager.SetupLog.Count();
                Assert.True(countTracesAfterStop > countTracesAfterStart);
            }
        }

        [Fact(Skip = "Not implemented")]
        public void LogManager_internal_operations_can_be_logged_to_another_target()
        {}

        [Fact]
        public void FinalizerCausesQueuedLogsToFlush()
        {
            var setupLog = new SetupLog();

            // Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
            const int opDelayMs = 5;
            var slowLogWriter = new SlowTestLogWriter<MessageEntry>(setupLog, opDelayMs, false);
            const int countLoggingThreads = 5;
            const int countMessagesPerThread = 5;
            const int expectedEntryCount = countLoggingThreads * countMessagesPerThread;

            // Run the test code in a delegate so that local variable references can be GCed
            Action logTestMessages = () =>
                                     {
                                         var logManager = new LogManager(new LogManagerConfig(), setupLog);
                                         logManager.Config.UseLogWriter(slowLogWriter).BackgroundLogging = true;
                                         var entryWriter = logManager.GetEntryWriter<MessageEntry>();

                                         ExampleHelper.LogTestMessagesInParallel(entryWriter, countMessagesPerThread, countLoggingThreads);

                                         // Key point: The LogManager is never disposed, and it has a number of queued
                                         // entries that haven't been written at this point.
                                     };
            logTestMessages();

            Assert.True(slowLogWriter.Count < expectedEntryCount);

            // Force a GC cyle, and wait for finalizers to complete.
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Equal(expectedEntryCount, slowLogWriter.Count);

            // When the finalizer is called, an error is logged to the SetupLog.
            Assert.True(setupLog.Any(traceEntry => (traceEntry.TraceLevel == TraceLevel.Error) && (traceEntry.Message.StartsWith("In finalizer "))));
        }

        [Fact]
        public void GetLogWriter_StartsLogManager()
        {
            var logManager = new LogManager();
            var consoleLogWriterConfig = logManager.Config.UseConsole();

            // LogManager.GetLogWriter starts the LogManager
            Assert.False(logManager.IsStarted);
            Assert.NotNull(logManager.GetLogWriter(consoleLogWriterConfig));
            Assert.True(logManager.IsStarted);
            Assert.True(logManager.IsHealthy);
        }

        [Fact]
        public void MissingLogWriterConfigThrows()
        {
            var logManager = new LogManager();

            // LogManager.GetLogWriter throws on no match
            var missingConfig = new ListLogWriterConfig<TraceEntry>();
            Assert.Throws<KeyNotFoundException>(() => logManager.GetLogWriter(missingConfig));
            Assert.True(logManager.IsHealthy);

            // LogManager.GetEntryWriter also throws
            Assert.Throws<KeyNotFoundException>(() => logManager.GetEntryWriter<TraceEntry>(missingConfig));
        }

    }

}
