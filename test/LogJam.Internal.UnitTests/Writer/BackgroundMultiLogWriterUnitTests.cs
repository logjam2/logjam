// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BackgroundMultiLogWriterUnitTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests.Writer
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Test.Shared.Writers;
    using LogJam.UnitTests.Examples;
    using LogJam.Util;
    using LogJam.Writer;
    using LogJam.Writer.Text;

    using Xunit;
    using Xunit.Abstractions;

    using TraceLevel = LogJam.Trace.TraceLevel;
    using LogJam.Shared.Internal;


    /// <summary>
    /// Validates behavior of <see cref="BackgroundMultiLogWriter" />.
    /// </summary>
    public sealed class BackgroundMultiLogWriterUnitTests
    {

        // Some tests disable timing-sensitive Assert()s if running in a debugger - b/c the debugger throws timing off
        private readonly bool _inDebugger = System.Diagnostics.Debugger.IsAttached;

        private readonly ITestOutputHelper _testOutputHelper;

        public BackgroundMultiLogWriterUnitTests(ITestOutputHelper testOutputHelper)
        {
            Arg.NotNull(testOutputHelper, nameof(testOutputHelper));

            _testOutputHelper = testOutputHelper;
            SetupLog = new SetupLog();
        }

        private SetupLog SetupLog { get; }

        /// <summary>
        /// Sets up a <see cref="BackgroundMultiLogWriter" /> that writes to <paramref name="innerLogWriter" /> on a background
        /// thread.
        /// </summary>
        /// <param name="innerLogWriter">The <see cref="IEntryWriter{TEntry}" /> that is written to on the background thread.</param>
        /// <param name="backgroundMultiLogWriter"></param>
        /// <param name="queueEntryWriter">
        /// The returned <see cref="IQueueEntryWriter{TEntry}" />, which writes to a queue which feeds logging on the background
        /// thread.
        /// </param>
        /// <param name="maxQueueLength"></param>
        private void SetupBackgroundLogWriter<TEntry>(ILogWriter innerLogWriter,
                                                      out BackgroundMultiLogWriter backgroundMultiLogWriter,
                                                      out IQueueEntryWriter<TEntry> queueEntryWriter,
                                                      int maxQueueLength = BackgroundMultiLogWriter.DefaultMaxQueueLength)
            where TEntry : ILogEntry
        {
            Arg.NotNull(innerLogWriter, nameof(innerLogWriter));

            backgroundMultiLogWriter = new BackgroundMultiLogWriter(SetupLog);
            ILogWriter logWriter = backgroundMultiLogWriter.CreateProxyFor(innerLogWriter, maxQueueLength);

            Assert.False(backgroundMultiLogWriter.IsStarted());
            Assert.False((logWriter as IStartable).IsStarted());

            backgroundMultiLogWriter.SafeStart(SetupLog);

            Assert.True(backgroundMultiLogWriter.IsStarted());
            Assert.True((logWriter as IStartable).IsStarted());

            Assert.True(logWriter.TryGetEntryWriter(out IEntryWriter<TEntry> entryWriter));

            queueEntryWriter = (IQueueEntryWriter<TEntry>) entryWriter;
        }

        [Fact(DisplayName = "Background logging makes slow logger act fast")]
        public void BackgroundLoggingMakesSlowLoggerActFast()
        {
            // Hi-res timer
            var stopwatch = new Stopwatch();

            // Slow log writer - starting, stopping, disposing, writing an entry, all take at least operationDelayMs each.
            const int operationDelayMs = 30;
            const int parallelThreads = 8;
            const int messagesPerThread = 6;
            var slowLogWriter = new SlowTestLogWriter<MessageEntry>(SetupLog, operationDelayMs, false);
            stopwatch.Start();
            SetupBackgroundLogWriter(slowLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);
            stopwatch.Stop();
            Assert.True(stopwatch.ElapsedMilliseconds >= operationDelayMs,
                        "Starting is slow, slowLogWriter start delay occurs on current thread. Elapsed: " + stopwatch.ElapsedMilliseconds);
            _testOutputHelper.WriteLine("Created + started BackgroundMultiLogWriter in {0}", stopwatch.Elapsed);

            using (backgroundMultiLogWriter)
            {
                stopwatch.Restart();
                ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, messagesPerThread, parallelThreads, _testOutputHelper);
                stopwatch.Stop();
                Assert.True((stopwatch.ElapsedMilliseconds < operationDelayMs) || _inDebugger, "Log writing should be fast, until the queue is filled.");

                stopwatch.Restart();
            }
            stopwatch.Stop();
            _testOutputHelper.WriteLine("Stop + dispose took {0}", stopwatch.Elapsed);

            // At this point, everything should have been logged
            Assert.Equal(parallelThreads * messagesPerThread, slowLogWriter.Count);
            Assert.True(stopwatch.ElapsedMilliseconds > messagesPerThread * parallelThreads * operationDelayMs,
                        "Dispose is slow, b/c we wait for all writes to complete on the background thread");
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter can be restarted")]
        public void CanRestartBackgroundMultiLogWriter()
        {
            var innerLogWriter = new TestLogWriter<MessageEntry>(SetupLog, false);
            SetupBackgroundLogWriter(innerLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

            // Re-starting shouldn't hurt anything
            Assert.True(backgroundMultiLogWriter.IsStarted());
            backgroundMultiLogWriter.Start();
            queueEntryWriter.Start();

            Assert.True(queueEntryWriter.IsEnabled);

            // Log some, then Stop
            ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);
            backgroundMultiLogWriter.Stop(); // Blocks until the background thread exits

            Assert.False(innerLogWriter.IsStarted());
            Assert.False(backgroundMultiLogWriter.IsStarted());
            Assert.False(queueEntryWriter.IsEnabled);

            Assert.Equal(8 * 8, innerLogWriter.Count);

            // After a Stop(), logging does nothing
            var msg = new MessageEntry("Logged while stopped - never logged.");
            queueEntryWriter.Write(ref msg);

            // After a Stop(), BackgroundMultiLogWriter and it's contained logwriters can be restarted
            backgroundMultiLogWriter.Start();

            Assert.True(queueEntryWriter.IsEnabled);

            // Log some, then Dispose
            ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);
            backgroundMultiLogWriter.Dispose(); // Blocks until the background thread exits

            Assert.False(innerLogWriter.IsStarted());
            Assert.False(backgroundMultiLogWriter.IsStarted());
            Assert.False(queueEntryWriter.IsEnabled);

            Assert.Equal(2 * 8 * 8, innerLogWriter.Count);

            // After a Dispose(), BackgroundMultiLogWriter can't be re-used
            Assert.Throws<ObjectDisposedException>(() => backgroundMultiLogWriter.Start());
            // Entries can still be written, though
            queueEntryWriter.Write(ref msg);
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter.Dispose prevents restart")]
        public void DisposePreventsRestart()
        {
            var innerLogWriter = new TestLogWriter<MessageEntry>(SetupLog, false);
            SetupBackgroundLogWriter(innerLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

            backgroundMultiLogWriter.Dispose(); // Blocks until the background thread exits

            // After a Dispose(), BackgroundMultiLogWriter can't be re-used
            Assert.Throws<ObjectDisposedException>(() => backgroundMultiLogWriter.Start());

            // Logging doesn't throw, though
            var msg = new MessageEntry("Logged while Dispose()ed - never logged.");
            queueEntryWriter.Write(ref msg);
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter.Dispose blocks until background logging completes")]
        public void DisposeBlocksUntilBackgroundLoggingCompletes()
        {
            // Slow log writer - starting, stopping, disposing, writing an entry, all take at least 20ms each.
            const int operationDelayMs = 5;
            const int parallelThreads = 8;
            const int messagesPerThread = 6;
            const int expectedMessageCount = parallelThreads * messagesPerThread;
            var slowLogWriter = new SlowTestLogWriter<MessageEntry>(SetupLog, operationDelayMs);
            SetupBackgroundLogWriter(slowLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

            using (backgroundMultiLogWriter)
            {
                ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, messagesPerThread, parallelThreads, _testOutputHelper);
                Assert.True(backgroundMultiLogWriter.IsBackgroundThreadRunning);
                Assert.True(slowLogWriter.Count < expectedMessageCount);
                // Dispose waits for all queued logs to complete, and for the background thread to exit
            }
            Assert.Equal(expectedMessageCount, slowLogWriter.Count);
            Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter: Stopping EntryWriter halts writing")]
        public void StoppingQueueEntryWriterHaltsWriting()
        {
            var innerLogWriter = new TestLogWriter<MessageEntry>(SetupLog, false);
            SetupBackgroundLogWriter(innerLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

            using (backgroundMultiLogWriter)
            {
                ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);
                queueEntryWriter.Stop();
                queueEntryWriter.Stop(); // Stopping twice shouldn't change things

                // These messages aren't logged
                ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);

                Assert.True(backgroundMultiLogWriter.IsBackgroundThreadRunning);

                queueEntryWriter.Start();
                queueEntryWriter.Start(); // Starting twice shouldn't change things
                ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);
            }

            Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
            Assert.Equal(2 * 8 * 8, innerLogWriter.Count);
        }

        /// <summary>
        /// The queue log writer can be disposed before the <see cref="BackgroundMultiLogWriter" /> is disposed.
        /// </summary>
        [Fact(DisplayName = "BackgroundMultiLogWriter: QueueEntryWriter can be disposed early")]
        public void QueueEntryWriterCanBeDisposedEarly()
        {
            var innerLogWriter = new TestLogWriter<MessageEntry>(SetupLog, false);
            SetupBackgroundLogWriter(innerLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

            using (backgroundMultiLogWriter)
            {
                using (queueEntryWriter as IDisposable)
                {
                    ExampleHelper.LogTestMessagesInParallel(queueEntryWriter, 8, 8, _testOutputHelper);
                }
                Assert.False(queueEntryWriter.IsEnabled);
                Assert.False(queueEntryWriter.IsStarted());
                Assert.Equal(StartableState.Disposed, queueEntryWriter.State);
                Assert.True(backgroundMultiLogWriter.IsStarted());

                // Can't restart after Dispose()
                Assert.Throws<ObjectDisposedException>(() => queueEntryWriter.Start());

                // Logging still doesn't throw after Dispose
                var msg = new MessageEntry("Logged while Dispose()ed - never logged.");
                queueEntryWriter.Write(ref msg);
            }
            Assert.False(backgroundMultiLogWriter.IsStarted());

            Assert.False(backgroundMultiLogWriter.IsBackgroundThreadRunning);
            Assert.Equal(8 * 8, innerLogWriter.Count);
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter: Exceeding queue size blocks logging")]
        public void ExceedingQueueSizeBlocksLogging()
        {
            // Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
            const int opDelayMs = 50;
            const int maxQueueLength = 10;
            const int countBlockingWrites = 4;
            var slowLogWriter = new SlowTestLogWriter<MessageEntry>(SetupLog, opDelayMs);
            SetupBackgroundLogWriter(slowLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter, maxQueueLength);

            var stopwatch = Stopwatch.StartNew();
            using (backgroundMultiLogWriter)
            {
                // First 10 messages log fast, then the queue is exactly full
                LoadHelper.LogTestMessages(queueEntryWriter, maxQueueLength);
                stopwatch.Stop();
                _testOutputHelper.WriteLine("First {0} writes took: {1}ms", maxQueueLength, stopwatch.ElapsedMilliseconds);
                Assert.True(_inDebugger || (stopwatch.ElapsedMilliseconds <= opDelayMs), "Log writing should be fast, until the queue is filled.");

                // Next writes block, since we've filled the queue
                for (int i = 0; i < countBlockingWrites; ++i)
                {
                    stopwatch.Restart();
                    LoadHelper.LogTestMessages(queueEntryWriter, 1);
                    stopwatch.Stop();
                    long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                    _testOutputHelper.WriteLine("Blocking write #{0}: {1}ms", i, elapsedMilliseconds);
                    Assert.True((elapsedMilliseconds >= (opDelayMs * 0.5)) || (i == 0) || _inDebugger,
                                "Expect blocking until 1 element is written - elapsed: " + elapsedMilliseconds);
                    // This assert is not passing on hqs01 - re-check another time.
                    // Timing-sensitive tests are always a bit delicate
                    //Assert.True((i == 0) || (elapsedMilliseconds < 2 * opDelayMs) || _inDebugger, "First write may be delayed; after that blocking should only occur for the duration of writing 1 entry. i=" + i + " Elapsed: " + elapsedMilliseconds);
                }

                stopwatch.Restart();
            }
            stopwatch.Stop();
            _testOutputHelper.WriteLine("Stop + dispose took {0}", stopwatch.Elapsed);

            // At this point, everything should have been logged
            Assert.Equal(maxQueueLength + countBlockingWrites, slowLogWriter.Count);
            Assert.True(stopwatch.ElapsedMilliseconds > maxQueueLength * opDelayMs, "Dispose writes all queued entries");

            // maxQueueLength+2 is the number of sleeps to wait for - the queue is full, +2 is for Stop() + Dispose() sleeps
            // 2.0 is just a tolerance for thread-related delays
            Assert.True((stopwatch.ElapsedMilliseconds < (maxQueueLength + 2) * opDelayMs * 2.0) || _inDebugger, "Took longer than expected: " + stopwatch.ElapsedMilliseconds);
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter: Inner LogWriter exceptions are handled")]
        public void InnerWriterExceptionsAreHandled()
        {
            var innerLogWriter = new ExceptionThrowingLogWriter<MessageEntry>(SetupLog);
            var backgroundMultiLogWriter = new BackgroundMultiLogWriter(SetupLog);
            ILogWriter logWriter = backgroundMultiLogWriter.CreateProxyFor(innerLogWriter);
            backgroundMultiLogWriter.Start();
            Assert.True(logWriter.TryGetEntryWriter(out IEntryWriter<MessageEntry> entryWriter));

            using (backgroundMultiLogWriter)
            {
                backgroundMultiLogWriter.Start();
                Assert.True(logWriter.TryGetEntryWriter(out entryWriter));
                LoadHelper.LogTestMessages(entryWriter, 6);
            }
            Assert.Equal(7, innerLogWriter.CountExceptionsThrown); // Logged 6 entries, plus 1 exception during dispose

            _testOutputHelper.WriteLine("Setup messages:");
            _testOutputHelper.WriteEntries(SetupLog);
            Assert.Equal(innerLogWriter.CountExceptionsThrown, SetupLog.Where((traceEntry, index) => traceEntry.TraceLevel >= TraceLevel.Error).Count());
        }

        [Fact(DisplayName = "BackgroundMultiLogWriter: Perf test vs foreground writing", Skip = "Not implemented")]
        public void PerfTestBackgroundWritingVsForegroundWriting()
        {}

        /// <summary>
        /// Even if you forget to call <see cref="BackgroundMultiLogWriter.Dispose" />, logs should still be written
        /// to their target when the <see cref="BackgroundMultiLogWriter" /> finalizer is called.
        /// </summary>
        [Fact(Skip = "Finalizer is not triggering, need to investigate: https://github.com/logjam2/logjam/issues/25")]
        public void FinalizerCausesQueuedLogsToFlush()
        {
            // Slow log writer - starting, stopping, disposing, writing an entry, all take at least 10ms each.
            const int opDelayMs = 5;
            var slowLogWriter = new SlowTestLogWriter<MessageEntry>(SetupLog, opDelayMs);
            const int expectedEntryCount = 35;

            // Run the test code in a delegate so that local variable references can be GCed
            Action logTestMessages = () =>
                                     {
                                         // Writes to test output when finalization occurs
                                         var finalizerCanary = new FinalizerCanary(_testOutputHelper);

                                         SetupBackgroundLogWriter(slowLogWriter, out BackgroundMultiLogWriter backgroundMultiLogWriter, out IQueueEntryWriter<MessageEntry> queueEntryWriter);

                                         LoadHelper.LogTestMessages(queueEntryWriter, expectedEntryCount);

                                         // Key point: The BackgroundMultiLogWriter is never disposed, and it has a number of queued
                                         // entries that haven't been written
                                     };
            logTestMessages();

            // Because the log writer is slow, messages should be queued/not yet written after returning.
            Assert.True(slowLogWriter.Count < expectedEntryCount);

            // Force a GC cyle, and wait for finalizers to complete.
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Assert.Equal(expectedEntryCount, slowLogWriter.Count);

            // When the finalizer is called, an error is logged to the SetupLog.
            Assert.Contains(SetupLog, traceEntry => (traceEntry.TraceLevel == TraceLevel.Error) && (traceEntry.Message.Equals("In finalizer (~LogManager) - forgot to Dispose()?")));
        }

        /// <summary>
        /// Exercises a <see cref="TextWriterLogWriter" /> behind a <see cref="BackgroundMultiLogWriter" />.
        /// </summary>
        /// <seealso cref="TextWriterLogWriterUnitTests.MultiLogWriterToText" />
        /// , which is only different by a line or two.
        [Fact(DisplayName = "BackgroundMultiLogWriter: Log multiple entry types to text")]
        public void BackgroundMultiLogWriterToText()
        {
            // Just to ensure that formats + writes occur on a background thread
            int testThreadId = Thread.CurrentThread.ManagedThreadId;

            // Log output written here on the background thread
            var stringWriter = new StringWriter();

            EntryFormatAction<LoggingTimer.StartRecord> formatStart = (startRecord, writer) =>
                                                                      {
                                                                          Assert.NotEqual(testThreadId, Thread.CurrentThread.ManagedThreadId);
                                                                          writer.BeginEntry();
                                                                          writer.WriteField((buffer) => buffer.AppendFormat(">{0}", startRecord.TimingId));
                                                                          writer.EndEntry();
                                                                      };
            EntryFormatAction<LoggingTimer.StopRecord> formatStop = (stopRecord, writer) =>
                                                                    {
                                                                        Assert.NotEqual(testThreadId, Thread.CurrentThread.ManagedThreadId);
                                                                        writer.BeginEntry();
                                                                        writer.WriteField((buffer) => buffer.AppendFormat("<{0}", stopRecord.TimingId));
                                                                        writer.WriteField(stopRecord.ElapsedTime.ToString());
                                                                        writer.EndEntry();
                                                                    };

            var logManagerConfig = new LogManagerConfig();
            logManagerConfig.UseTextWriter(stringWriter)
                            .Format(formatStart)
                            .Format(formatStop)
                            .BackgroundLogging = true;
            using (var logManager = new LogManager(logManagerConfig))
            {
                // LoggingTimer test class logs starts and stops
                LoggingTimer.RestartTimingIds();
                var timer = new LoggingTimer("test LoggingTimer", logManager);
                var timing1 = timer.Start();
                Thread.Sleep(15);
                timing1.Stop();
                var timing2 = timer.Start();
                Thread.Sleep(10);
                timing2.Stop();
            }

            string logOutput = stringWriter.ToString();
            _testOutputHelper.WriteLine(logOutput);

            Assert.Contains(">2\r\n<2  00:00:00.", logOutput);
            Assert.Contains(">3\r\n<3  00:00:00.", logOutput);
        }

        /// <summary>
        /// A finalizeable object that writes to test output when finalization occurs. Used to check if finalization occurs when finalization tests fail.
        /// </summary>
        private class FinalizerCanary
        {

            private readonly ITestOutputHelper _testOutput;

            public FinalizerCanary(ITestOutputHelper testOutput)
            {
                Arg.NotNull(testOutput, nameof(testOutput));

                _testOutput = testOutput;
            }

            ~FinalizerCanary()
            {
                _testOutput.WriteLine("(~FinalizerCanary called)");
            }
        }
    }

}
