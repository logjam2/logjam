// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestOutputLoggingTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.XUnit2.UnitTests
{
    using LogJam.Trace;
    using LogJam.Trace.Config;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// General tests for logging to test output.
    /// </summary>
    public sealed class TestOutputLoggingTests
    {

        /// <summary>
        /// If the <see cref="ITestOutputHelper"/> is not set, things still work.
        /// </summary>
        [Fact]
        public void TestOutputDoesNotHaveToBeSet()
        {
            var testOutputLogWriterConfig = new TestOutputLogWriterConfig() // Important: No ITestOutputHelper is set
            {
                BackgroundLogging = false
            };
            using (var traceManager = new TraceManager(testOutputLogWriterConfig))
            {
                var entryWriter = traceManager.LogManager.GetEntryWriter<TraceEntry>();
                var entry = new TraceEntry(nameof(TestOutputLoggingTests), TraceLevel.Info, "test");
                entryWriter.Write(ref entry);

                Assert.False(traceManager.SetupLog.HasAnyExceeding(TraceLevel.Warn));
            }
        }

    }
}