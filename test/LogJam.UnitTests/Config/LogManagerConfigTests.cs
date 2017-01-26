// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerConfigTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2. 
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Config
{
    using System;
    using System.IO;

    using LogJam.Config;
    using LogJam.Trace;
    using LogJam.Trace.Format;
    using LogJam.Writer;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Exercises <see cref="LogManagerConfig" />.
    /// </summary>
    public sealed class LogManagerConfigTests
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public LogManagerConfigTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void DefaultLogManagerHasEmptyConfig()
        {
            using (var logManager = new LogManager())
            {
                Assert.Empty(logManager.Config.Writers);
                Assert.False(logManager.IsStarted());
                Assert.True(logManager.IsHealthy);
            }
        }

        /// <summary>
        /// Covers cases like "the log file couldn't be opened".
        /// </summary>
        [Fact]
        public void LogWriterCreateExceptionIsHandled()
        {
            Func<TextWriter> throwOnCreate = () => { throw new AccessViolationException("Testing what happens when access is denied"); };

            using (var logManager = new LogManager())
            {
                TextWriterLogWriterConfig logWriterConfig = logManager.Config.UseTextWriter(throwOnCreate);
                logWriterConfig.Format(new DefaultTraceFormatter());
                Assert.True(logManager.IsHealthy);

                // Exception is thrown during Start()
                logManager.Start();
                Assert.False(logManager.IsHealthy);
                _testOutputHelper.WriteEntries(logManager.SetupLog);

                ILogWriter logWriter;
                Assert.False(logManager.TryGetLogWriter(logWriterConfig, out logWriter));

                var traceEntryWriter = logManager.GetEntryWriter<TraceEntry>();
                Assert.NotNull(traceEntryWriter);
                Assert.IsType<NoOpEntryWriter<TraceEntry>>(traceEntryWriter);
            }
        }

    }

}
