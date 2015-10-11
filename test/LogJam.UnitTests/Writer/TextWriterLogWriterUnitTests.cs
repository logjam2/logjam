// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextWriterLogWriterUnitTests.cs">
// Copyright (c) 2011-2015 https://github.com/logjam2.  
// </copyright>
// Licensed under the <a href="https://github.com/logjam2/logjam/blob/master/LICENSE.txt">Apache License, Version 2.0</a>;
// you may not use this file except in compliance with the License.
// --------------------------------------------------------------------------------------------------------------------


namespace LogJam.UnitTests.Writer
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;

    using LogJam.Format;
    using LogJam.UnitTests.Examples;
    using LogJam.Writer;

    using Xunit;
    using Xunit.Abstractions;


    /// <summary>
    /// Exercises <see cref="TextWriterLogWriter" />.
    /// </summary>
    public sealed class TextWriterLogWriterUnitTests
    {

        private readonly ITestOutputHelper _testOutputHelper;

        public TextWriterLogWriterUnitTests(ITestOutputHelper testOutputHelper)
        {
            Contract.Requires<ArgumentNullException>(testOutputHelper != null);

            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void MultiLogWriterToText()
        {
            // Log output written here
            var stringWriter = new StringWriter();

            var setupTracerFactory = new SetupLog();
            FormatAction<LoggingTimer.StartRecord> formatStart = (startRecord, writer) => writer.WriteLine(">{0}", startRecord.TimingId);
            FormatAction<LoggingTimer.StopRecord> formatStop = (stopRecord, writer) => writer.WriteLine("<{0} {1}", stopRecord.TimingId, stopRecord.ElapsedTime);
            var multiLogWriter = new TextWriterLogWriter(stringWriter, setupTracerFactory)
                .AddFormat(formatStart)
                .AddFormat(formatStop);

            using (var logManager = new LogManager(multiLogWriter))
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

            Assert.Contains(">2\r\n<2 00:00:00.", logOutput);
            Assert.Contains(">3\r\n<3 00:00:00.", logOutput);
        }

    }

}
