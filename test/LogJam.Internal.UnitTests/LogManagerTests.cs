// // --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogManagerTests.cs">
// Copyright (c) 2011-2016 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.Internal.UnitTests
{
    using System.Collections.Generic;

    using LogJam.Config;
    using LogJam.Internal.UnitTests.Examples;
    using LogJam.Trace;
    using LogJam.XUnit2;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Verifies internal <see cref="LogManager"/> methods.
    /// </summary>
    public sealed class LogManagerTests
    {

        private readonly ITestOutputHelper _testOutput;

        public LogManagerTests(ITestOutputHelper testOutput, ITestOutputHelper testOutput1)
        {
            _testOutput = testOutput1;
        }

        [Fact]
        public void IsRestartNeeded_Works()
        {
            using (var logManager = new LogManager())
            {
                logManager.Config.UseList(new List<TraceEntry>());
                logManager.Config.UseList(new List<MessageEntry>());

                Assert.True(logManager.IsRestartNeeded());

                // Starts the LogManager
                var traceWriter = logManager.GetEntryWriter<TraceEntry>();

                Assert.False(logManager.IsRestartNeeded());

                // Add a new LogWriter
                logManager.Config.UseTestOutput(_testOutput);

                // Now a restart is justified.
                Assert.True(logManager.IsRestartNeeded());
            }
        }

    }

}