// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SynchronizedLogWriterTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Writer;

    using Xunit;
    using Xunit.Sdk;


    /// <summary>
    /// Validates that <see cref="SynchronizingProxyLogWriter" /> works as expected.
    /// </summary>
    public sealed class SynchronizedLogWriterTests
    {

        private static readonly TimeSpan s_defaultWriteDelay = TimeSpan.FromMilliseconds(1);
        private const int c_defaultParallelThreads = 40;
        private const int c_defaultEntriesPerThread = 20;

        /// <summary>
        /// Since synchronization isn't used, validation should easily fail.
        /// </summary>
        [Fact]
        public void UnsynchronizedLogWriterFailsValidation()
        {
            ValidateSyncedLogWriter validatingLogWriter;
            using (var logManager = new LogManager())
            {
                // This LogWriter throws Assert exceptions if writes aren't properly synchronized
                validatingLogWriter = new ValidateSyncedLogWriter(logManager.SetupTracerFactory, s_defaultWriteDelay);
                var logWriterConfig = logManager.Config.UseLogWriter(validatingLogWriter);
                logWriterConfig.Synchronized = false;

                AggregateException aggregateException = null;
                try
                {
                    RunParallelWrites(logManager, c_defaultParallelThreads, c_defaultEntriesPerThread);
                }
                catch (AggregateException aggExcp)
                {
                    aggregateException = aggExcp;
                }

                // Expected: some Asserts will fail
                Assert.NotNull(aggregateException);
                Assert.InRange(aggregateException.InnerExceptions.Count, c_defaultParallelThreads / 4, c_defaultParallelThreads);
                Assert.True(aggregateException.InnerExceptions.All(excp => excp is AssertActualExpectedException));
            }

            Assert.NotEqual(c_defaultParallelThreads * c_defaultEntriesPerThread, validatingLogWriter.WritesCompleted);
        }

        /// <summary>
        /// By default, LogWriters are synchronized if they don't self-synchronize; and it works.
        /// </summary>
        [Fact]
        public void DefaultSynchronizeWorks()
        {
            ValidateSyncedLogWriter validatingLogWriter;
            using (var logManager = new LogManager())
            {
                validatingLogWriter = new ValidateSyncedLogWriter(logManager.SetupTracerFactory, s_defaultWriteDelay);
                var logWriterConfig = logManager.Config.UseLogWriter(validatingLogWriter);
                Assert.True(logWriterConfig.Synchronized);

                RunParallelWrites(logManager, c_defaultParallelThreads, c_defaultEntriesPerThread);
            }

            Assert.Equal(c_defaultParallelThreads * c_defaultEntriesPerThread, validatingLogWriter.WritesCompleted);
        }

        /// <summary>
        /// By default, LogWriters are synchronized if they don't self-synchronize; and it works.
        /// </summary>
        [Fact]
        public void BackgroundLoggingSynchronizes()
        {
            ValidateSyncedLogWriter validatingLogWriter;
            using (var logManager = new LogManager())
            {
                validatingLogWriter = new ValidateSyncedLogWriter(logManager.SetupTracerFactory, s_defaultWriteDelay);
                var logWriterConfig = logManager.Config.UseLogWriter(validatingLogWriter);
                logWriterConfig.BackgroundLogging = true;

                RunParallelWrites(logManager, c_defaultParallelThreads, c_defaultEntriesPerThread);
            }

            Assert.Equal(c_defaultParallelThreads * c_defaultEntriesPerThread, validatingLogWriter.WritesCompleted);
        }

        /// <summary>
        /// Creates parallel load for verifying synchronization.
        /// </summary>
        /// <param name="logManager"></param>
        /// <param name="parallelThreads"></param>
        /// <param name="entriesPerThread"></param>
        internal void RunParallelWrites(LogManager logManager, int parallelThreads, int entriesPerThread)
        {
            var entryWriter = logManager.GetEntryWriter<CounterLogEntry>();
            Action loggingFunc = () =>
                                 {
                                     for (int i = 0; i < entriesPerThread; ++i)
                                     {
                                         CounterLogEntry entry = new CounterLogEntry(i);
                                         entryWriter.Write(ref entry);
                                     }
                                 };
            Parallel.Invoke(Enumerable.Repeat(loggingFunc, parallelThreads).ToArray());
        }


        /// <summary>
        /// A simple test log entry. The contents aren't actually used.
        /// </summary>
        public struct CounterLogEntry : ILogEntry
        {

            private int _counter;

            public CounterLogEntry(int counter)
            {
                _counter = counter;
            }

            public int Counter { get { return _counter; } set { _counter = value; } }

        }


        /// <summary>
        /// An <see cref="ILogWriter" /> that validates that calls to it are correctly externally sychronized.
        /// </summary>
        public class ValidateSyncedLogWriter : SingleEntryTypeLogWriter<CounterLogEntry>
        {

            private readonly TimeSpan _writeDelay;
            private int _startWriteCounter, _endWriteCounter;

            public ValidateSyncedLogWriter(ITracerFactory setupTracerFactory, TimeSpan writeDelay)
                : base(setupTracerFactory)
            {
                _writeDelay = writeDelay;

                _startWriteCounter = 0;
                _endWriteCounter = 0;
            }

            public int WritesCompleted { get { return _endWriteCounter; } }

            public bool WritesSynchronized { get { return _endWriteCounter == _startWriteCounter; } }

            public override void Write(ref CounterLogEntry entry)
            {
                _startWriteCounter++;
                Thread.Sleep(_writeDelay);
                Assert.True(_endWriteCounter == _startWriteCounter - 1);
                _endWriteCounter++;
                Assert.True(WritesSynchronized);
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    Assert.True(_endWriteCounter == _startWriteCounter);
                }
            }

        }

    }

}
